namespace DeepSeekClient.Models
{
    internal class ConfigurationModel
    {
        public string ConfigUri { get; set; } = "https://api.deepseek.com/chat/completions";
        public string ConfigKey { get; set; } = string.Empty;

        public string ConfigTheme { get; set; } = "Dark";
        public string ConfigLanguage { get; set; } = "zh_CN";
    }
}