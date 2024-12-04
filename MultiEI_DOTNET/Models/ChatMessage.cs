// Models/ChatMessage.cs
using Newtonsoft.Json;

namespace MultiEI.Models
{
    public class ChatMessage
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
