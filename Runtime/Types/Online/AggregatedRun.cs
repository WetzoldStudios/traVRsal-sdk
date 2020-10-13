using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class AggregatedRun
    {
        public int run_count;
        public int tile_count_min;
        public int tile_count_max;
        public float tile_count_avg;
        public float tile_size_min;
        public float tile_size_max;
        public float tile_size_avg;
        public float distance_min;
        public float distance_max;
        public float distance_avg;
        public int points_min;
        public int points_max;
        public float points_avg;
        public float score_min;
        public float score_max;
        public float score_avg;
        public float time_taken_min;
        public float time_taken_max;
        public float time_taken_avg;
        public int player_damage_min;
        public int player_damage_max;
        public float player_damage_avg;
        public float accuracy_min;
        public float accuracy_max;
        public float accuracy_avg;
        public int targets_total_min;
        public int targets_total_max;
        public float targets_total_avg;
        public int targets_destroyed_min;
        public int targets_destroyed_max;
        public float targets_destroyed_avg;
        public int shots_fired_min;
        public int shots_fired_max;
        public float shots_fired_avg;
        public int shots_hit_min;
        public int shots_hit_max;
        public float shots_hit_avg;
        public int melee_hits_min;
        public int melee_hits_max;
        public float melee_hits_avg;
        public int melee_kills_min;
        public int melee_kills_max;
        public float melee_kills_avg;
        public int deaths_min;
        public int deaths_max;
        public float deaths_avg;
        public int rank_min;
        public int rank_max;
        public float rank_avg;
        public string date_min;
        public string date_max;

        public AggregatedRun()
        {
        }

        public override string ToString()
        {
            return $"Remote Aggregated Run Data {run_count}";
        }
    }
}