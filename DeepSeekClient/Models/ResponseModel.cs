namespace DeepSeekClient.Models
{
#pragma warning disable IDE1006 // 保持和响应大小写一致

    internal class ResponseModel
    {
        public string? id { get; set; }
        public ChoiceModel[]? choices { get; set; }
        public long? created { get; set; }
        public string? model { get; set; }
        public string? system_fingerprint { get; set; }

        //public string? object { get; set; } // object is reserved
    }

    internal class ChoiceModel
    {
        public DeltaModel? delta { get; set; }
        public string? finish_reason { get; set; }
        public int? index { get; set; }
    }

    internal class DeltaModel
    {
        public string? role { get; set; }
        public string? content { get; set; }
        public string? reasoning_content { get; set; }
    }

#pragma warning restore IDE1006 // 保持和响应大小写一致
}