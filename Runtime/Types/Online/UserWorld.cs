using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class UserWorld
    {
        public enum WorldState
        {
            initial,
            preview,
            released
        }

        public int id;
        public string key;
        public string channel;
        public string min_app_version;
        public string cover_image;
        public string unity_version;
        public string world_json;
        public WorldState state;
        public byte is_private;
        public byte is_virtual;
        public byte is_featured;
        public byte is_community;
        public long android_size;
        public long pc_size;
        public long linux_size;
        public long stats_started;
        public long stats_aborted;
        public long stats_finished;
        public float stats_time_spent;
        public float stats_distance;
        public string last_uploaded_at;
        public string last_statechange_at;
        public string creator;
        public string creators;

        public override string ToString()
        {
            return $"Remote World Data ({key})";
        }
    }
}