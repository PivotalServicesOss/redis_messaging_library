namespace PivotalServices.Redis.Messaging
{
    public class Message
    {
        public Message(string id = null, string body = null)
        {
            Id = id;
            Body = body;
        }

        public string Id { get; set; }
        public string Body { get; set; }
    }
}