using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Run
    {
        public int id;
        public string player_id;
        public string player_name;
        public string world;
        public string world_file;
        public string world_date;
        public int run_reference;
        public int rank;

        public int tile_count;
        public float tile_size;
        public string app_version;
        public string unity_version;

        public float time_taken;
        public float distance;
        public float accuracy;
        public int shots_fired;
        public int shots_hit;
        public int melee_hits;
        public int melee_kills;
        public int points;
        public int deaths;
        public int player_damage;
        public int targets_total;
        public int targets_destroyed;
        public string custom_kpis;
        public float score;

        public string created_at;

        public Run()
        {
        }

        public override string ToString()
        {
            return $"Remote Run Data {world}";
        }
    }
}