using DeepSeekClient.Core;
using DeepSeekClient.Events;
using DeepSeekClient.Models;
using Newtonsoft.Json;
using Prism.Events;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace DeepSeekClient.Services
{
    internal class DeepSeekService : IService
    {
        private readonly IEventAggregator _event;
        private readonly ConversationCore _converCore;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly HttpClient _httpClient;

        public DeepSeekService(IEventAggregator eventAggregator, ConversationCore conversationCore)
        {
            _event = eventAggregator;
            _converCore = conversationCore;
            _jsonSettings = new() { NullValueHandling = NullValueHandling.Ignore };
            _httpClient = new() { Timeout = TimeSpan.FromSeconds(60) };
        }

        public async Task SendRequestAsync(string inputContent, ConfigurationModel configuration, CharacterModel character, CancellationToken cancelToken)
        {
            try
            {
                var _uri = configuration.ConfigUri;
                var _key = configuration.ConfigKey;
                var _contextLimit = character.CharContextLimit;
                var _isCustomApi = character.CustomApi;
                var _charId = character.CharId;
                var _charSet = character.CharSet;

                RequestModel requestModel = new()
                {
                    model = character.CharModel,
                    temperature = character.CharTemperature,
                    max_tokens = 8000,
                    stream = true // 是否启用流式传输
                };

                if (_isCustomApi)
                {
                    _uri = character.CharUri;
                    _key = character.CharKey;
                }

                if (!IsValidHttpUrl(_uri)) { throw new Exception("Uri is not valid!"); }

                MessageModel userMessage = new() { role = "user", content = inputContent };

                List<MessageModel> messageList = [];

                var parameters = (userMessage, _charId);
                await Task.Run(() =>
                {
                    _event.GetEvent<ConversationUpdatedEvent>().Publish(parameters);
                    _event.GetEvent<CollectionChangedEvent>().Publish();
                }, cancelToken).ConfigureAwait(false);

                messageList.Add(userMessage);

                var historyMessages = _converCore.ConversationLoad(_charId).Messages;

                bool skipStop = false;

                for (int i = historyMessages.Count - 1; i >= 0; i--)
                {
                    if (_contextLimit <= 0) { break; }
                    var msg = historyMessages[i];
                    if (msg.role == "stop") { skipStop = !skipStop; }
                    if (skipStop == true) { continue; }
                    if (msg.role != "user" && msg.role != "assistant") { continue; }
                    messageList.Add(new MessageModel() { role = msg.role, content = msg.content });
                    if (msg.role == "user") { _contextLimit--; }
                }

                if (!string.IsNullOrWhiteSpace(_charSet) && requestModel.model == "deepseek-chat")
                {
                    messageList.Add(new MessageModel()
                    {
                        role = "system",
                        content = _charSet
                    });
                }

                messageList.Reverse();

                requestModel.messages = [.. messageList];

                var request = new HttpRequestMessage(HttpMethod.Post, _uri);
                request.Headers.Add("Accept", "text/event-stream"); //流式传输
                request.Headers.Add("Authorization", $"Bearer {_key}");

                var requestBody = JsonConvert.SerializeObject(requestModel, _jsonSettings);

                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                messageList.Clear();

                MessageModel assistantMessage = new() { role = "assistant" };

                using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancelToken).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                using Stream streamContent = await response.Content.ReadAsStreamAsync(cancelToken).ConfigureAwait(false);
                using StreamReader reader = new(streamContent);
                while (!reader.EndOfStream && !cancelToken.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync(cancelToken).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(": keep-alive")) { continue; }
                    var objString = line.Substring(5).Trim();
                    if (objString.StartsWith("[DONE]")) { break; }

                    var obj = JsonConvert.DeserializeObject<ResponseModel>(objString);

                    MessageModel tempMessage = new()
                    {
                        role = "assistant",
                        content = obj?.choices?[0]?.delta?.content ?? string.Empty,
                        reasoning_content = obj?.choices?[0]?.delta?.reasoning_content ?? string.Empty
                    };

                    if (string.IsNullOrEmpty(tempMessage.content) && string.IsNullOrEmpty(tempMessage.reasoning_content)) { continue; }

                    parameters = (tempMessage, _charId);
                    await Task.Run(() =>
                    {
                        _event.GetEvent<ConversationUpdatedEvent>().Publish(parameters);
                    }, cancelToken);
                    await Task.Run(() =>
                    {
                        _event.GetEvent<CollectionChangedEvent>().Publish();
                    }, cancelToken);

                    assistantMessage.content += tempMessage.content;
                    assistantMessage.reasoning_content += tempMessage.reasoning_content;
                }

                messageList.Add(userMessage);
                messageList.Add(assistantMessage);

                MessageModel stampMessage = new() { role = "stamp", content = DateTime.Now.ToString("g") };
                messageList.Add(stampMessage);
                parameters = (stampMessage, _charId);
                await Task.Run(() =>
                {
                    _event.GetEvent<ConversationUpdatedEvent>().Publish(parameters);
                    _event.GetEvent<CollectionChangedEvent>().Publish();
                }, cancelToken).ConfigureAwait(false);

                if (cancelToken.IsCancellationRequested)
                {
                    MessageModel stopMessage = new() { role = "stop", content = "## Stop Generating! ##" };

                    messageList.Add(stopMessage);
                    messageList.Insert(0, stopMessage);

                    parameters = (stopMessage, _charId);
                    await Task.Run(() =>
                    {
                        _event.GetEvent<ConversationUpdatedEvent>().Publish(parameters);
                        _event.GetEvent<CollectionChangedEvent>().Publish();
                    }, cancelToken).ConfigureAwait(false);
                }

                historyMessages.AddRange(messageList);

                await Task.Run(() => _converCore.ConversationSave(new ConversationModel() { Messages = historyMessages }, _charId), cancelToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
            }
        }

        private static bool IsValidHttpUrl(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}