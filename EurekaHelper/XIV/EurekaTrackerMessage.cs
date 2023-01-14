using Newtonsoft.Json.Linq;

namespace EurekaHelper.XIV
{
    public class EurekaTrackerMessage
    {
        public EurekaTrackerMessage(bool setFirstValue, int messageId, string channel, string @event, JObject payload)
        {
            SetFirstValue = setFirstValue;
            MessageId = messageId;
            Channel = channel;
            Event = @event;
            Payload = payload;
        }

        public bool SetFirstValue { get; set; }
        public int MessageId { get; set; }
        public string Channel { get; set; }
        public string Event { get; set; }
        public JObject Payload { get; set; }

        public string ToMessage()
        {
            JArray array = new()
            {
                SetFirstValue ? "1" : null,
                MessageId.ToString(),
                Channel,
                Event,
                Payload == null ? "null" : Payload
            };

            return array.ToString();
        }
    }
}
