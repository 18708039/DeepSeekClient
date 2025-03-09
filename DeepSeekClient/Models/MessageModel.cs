using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekClient.Models
{
#pragma warning disable IDE1006 // 保持和请求大小写一致

    internal class MessageModel
    {
        public string content { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string? name { get; set; }
        public bool? prefix { get; set; }
        public string? reasoning_content { get; set; }
    }
}