using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Run
    {
        public enum ControlScheme
        {
            walking,
            smooth,
            teleport,
            treadmill
        }

        public int id;
        public string player_id;
        public string player_name;
        public int support_level;
        public string world;
        public string world_file;
        public string world_date;
        public string kpis;
        public string chapter;
        public string channel;

        public int run_reference;
        public int rank;

        public int tile_count_x;
        public int tile_count_y;
        public float tile_size;
        public string app_version;
        public string unity_version;
        public string world_version;
        public int world_version_code;

        public float time_taken;
        public float distance;
        public float accuracy;
        public int shots_fired;
        public int shots_hit;
        public int melee_hits;
        public int melee_kills;
        public int points;
        public int deaths;
        public int targets_total;
        public int targets_destroyed;
        public string custom_kpis;
        public float player_damage;
        public float score;
        public int is_valid;
        public ControlScheme controlScheme;

        public string created_at;

        public Run()
        {
        }

        public override string ToString()
        {
            return $"Remote Run Data ({world})";
        }
    }
}