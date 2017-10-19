using System.Collections.Generic;

namespace Bot.Models.Facebook
{
        public class Message
    {
        public string mid { get; set; }
        public int seq { get; set; }
        public bool is_echo { get; set; }
        public List<Attachment> attachments { get; set; }
    }
}
