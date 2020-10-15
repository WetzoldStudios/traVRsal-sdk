using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class PlayerInfo
    {
        public int postedchallenges_total;
        public int postedchallenges_beaten;
        public int postedchallenges_tries_success;
        public int postedchallenges_tries_failed;
        public int postedchallenges_tries_total;
        public int challenges_total;
        public int runs_total;
        public int challenges_beaten;
        public int challenges_best;
        public Run[] unbeatenchallenges_compatible;
        
        public PlayerInfo()
        {
        }

        public override string ToString()
        {
            return $"Remote player info data ({runs_total} runs)";
        }
    }
}