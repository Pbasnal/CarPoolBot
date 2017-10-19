namespace Bot.Models.Facebook
{
    public class Attachment
    {
        public string type { get; set; }
        public Payload payload { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }
}
