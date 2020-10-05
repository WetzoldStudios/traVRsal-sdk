using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Run
    {
        public int id;
        public string player_id;
        public string world;
        public string world_file;
        public string world_date;
        public long tile_count;
        public float tile_size;
        public string app_version;
        public string unity_version;
        public float time_taken;
        public float distance;
        public long shots_fired;
        public long shots_hit;
        public long melee_hits;
        public long melee_kills;
        public long points;
        public long deaths;
        public long player_damage;
        public long targets_total;
        public long targets_destroyed;
        public string custom_kpis;
        public float score;

        public override string ToString()
        {
            return $"Remote Run Data {world}";
        }
    }
}