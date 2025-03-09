namespace DeepSeekClient.Models
{
    internal class CharacterModel
    {
        public CharacterModel(string charId)
        {
            CharId = charId;
        }

        public string CharId { get; set; }
        public string CharName { get; set; } = string.Empty;
        public string CharSet { get; set; } = "You are a helpful assistant.";
        public double CharTemperature { get; set; } = 1.3;

        public bool CustomApi { get; set; } = false;
        public string CharUri { get; set; } = string.Empty;
        public string CharKey { get; set; } = string.Empty;
    }
}