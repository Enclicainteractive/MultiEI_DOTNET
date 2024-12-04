// Models/VoteKickRecord.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MultiEI.Models
{
    public class VoteKickRecord
    {
        [JsonProperty("targetPlayerId")]
        public string TargetPlayerId { get; set; }

        [JsonProperty("voters")]
        public HashSet<string> Voters { get; set; }

        [JsonProperty("lastVote")]
        public long LastVote { get; set; }

        [JsonProperty("votes")]
        public VoteCounts Votes { get; set; }

        public VoteKickRecord()
        {
            Voters = new HashSet<string>();
            Votes = new VoteCounts();
        }
    }

    public class VoteCounts
    {
        [JsonProperty("yes")]
        public int Yes { get; set; }

        [JsonProperty("no")]
        public int No { get; set; }
    }
}
