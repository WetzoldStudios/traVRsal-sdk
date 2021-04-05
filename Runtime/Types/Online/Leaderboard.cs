using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Leaderboard
    {
        public string aspect;
        public int ranked_players;
        public LeaderboardEntry player;
        public LeaderboardEntry[] player_bracket;
        public LeaderboardEntry[] top;
        public LeaderboardEntry[] top_merged;

        public Leaderboard()
        {
        }

        public override string ToString()
        {
            return $"Remote Leaderboard Data ({aspect}, {ranked_players} ranked players)";
        }
    }
}