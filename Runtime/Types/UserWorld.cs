using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class UserWorld
    {
        public enum WorldState
        {
            initial, preview, released
        }
        public int id;
        public string key;
        public string cover_image;
        public string unity_version;
        public string world_json;
        public WorldState state;
        public bool is_private;
        public bool is_virtual;
        public string last_uploaded_at;
        public string last_statechange_at;
    }
}