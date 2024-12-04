// Models/Instance.cs
using MultiEI.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MultiEI.Models
{
    public class Instance
    {
        [JsonProperty("pin")]
        public string Pin { get; set; }

        [JsonProperty("masterId")]
        public string MasterId { get; set; }

        [JsonProperty("world")]
        public string World { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("objects")]
        public Dictionary<string, InstanceObject> Objects { get; set; }

        public Instance()
        {
            Objects = new Dictionary<string, InstanceObject>();
        }
    }

    public class InstanceObject
    {
        [JsonProperty("position")]
        public Vector3 Position { get; set; }

        [JsonProperty("ownerId")]
        public string OwnerId { get; set; }
    }
}
