using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Player
    {
        public int id;
        public int user_id;
        public string login;
        public int accepted_terms_version;
        public string accepted_terms_date;
        public string player_id;
        public string oculus_id; // php will not convert biginteger correctly
        public string nickname;
        public float distance;

        public string device;
        public string app_version;
        public string unity_version;

        public string created_at;
        public string updated_at;

        public Player()
        {
        }

        public override string ToString()
        {
            return $"Remote Player Data {nickname}";
        }
    }
}