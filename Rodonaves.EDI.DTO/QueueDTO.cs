using Newtonsoft.Json;

namespace Rodonaves.EDI.DTO
{
    public class QueueDTO
    {
        public string Name { get; set; }
        public string Messages { get; set; }

        [JsonProperty ("messages_ready")]
        public string MessagesReady { get; set; }

        [JsonProperty ("messages_unacknowledged")]
        public string Unacknowledged { get; set; }
    }
}