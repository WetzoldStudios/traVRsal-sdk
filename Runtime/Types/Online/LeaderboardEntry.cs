using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string player_name;
        public string player_id;
        public float score;
        public int rank;
        public bool rank_gap;

        public int challenge_runs;
        public float challenge_distance;
        public float challenge_time;
        public int first;
        public int other_place;
        public int unsuccessful;
        public float best_speed;

        public LeaderboardEntry()
        {
        }

        public override string ToString()
        {
            return $"Remote Leaderboard Entry Data ({rank})";
        }
    }
}