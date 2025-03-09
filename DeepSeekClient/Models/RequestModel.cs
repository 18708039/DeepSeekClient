using System.Net.Http;

namespace DeepSeekClient.Models
{
#pragma warning disable IDE1006 // 保持和请求大小写一致

    internal class RequestModel
    {
        public MessageModel[] messages { get; set; } = [];
        public string model { get; set; } = "deepseek-chat";
        public double? frequency_penalty { get; set; }
        public int? max_tokens { get; set; }
        public double? presence_penalty { get; set; }
        public object? response_format { get; set; }
        public object? stop { get; set; }
        public bool? stream { get; set; }
        public object? stream_options { get; set; }
        public double? temperature { get; set; }
        public double? top_p { get; set; }
        public object[]? tools { get; set; }
        public object? tool_choice { get; set; }
        public bool? logprobs { get; set; }
        public int? top_logprobs { get; set; }
    }
}