using DeepSeekClient.Models;

namespace DeepSeekClient.Services
{
    internal interface IService : IDisposable
    {
        Task SendRequestAsync(string inputContent, ConfigurationModel configuration, CharacterModel character, CancellationToken cancelToken);
    }
}