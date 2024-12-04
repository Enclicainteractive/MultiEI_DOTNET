// Models/Player.cs
using MultiEI.Utilities;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiEI.Models
{
    public class Player
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; }

        [JsonProperty("cosmeticId")]
        public string CosmeticId { get; set; }

        [JsonProperty("location")]
        public Vector3 Location { get; set; }

        [JsonProperty("rotation")]
        public Vector3 Rotation { get; set; }

        [JsonProperty("voiceData")]
        public VoiceData VoiceData { get; set; }

        [JsonProperty("pickedItem")]
        public string PickedItem { get; set; }

        [JsonProperty("avatarElements")]
        public AvatarElements AvatarElements { get; set; }

        // Additional fields can be added here as needed
    }

    public class VoiceData
    {
        [JsonProperty("isTalking")]
        public bool IsTalking { get; set; }

        [JsonProperty("position")]
        public Vector3 Position { get; set; }
    }

    public class AvatarElements
    {
        [JsonProperty("elements")]
        public Dictionary<string, object> Elements { get; set; }

        public AvatarElements()
        {
            Elements = new Dictionary<string, object>();
        }
    }
}
