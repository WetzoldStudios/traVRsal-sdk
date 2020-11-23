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
        public string is_private;
        public string is_virtual;
        public string is_featured;
        public long android_size;
        public long pc_size;
        public string last_uploaded_at;
        public string last_statechange_at;
        public string creator;

        public override string ToString()
        {
            return $"Remote World Data {key}";
        }
    }
}