using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class ReplicaSpeech
    {
        public string uuid;
        public float generation_time;
        public string url;
        public float duration;
        public string speaker_id;
        public string txt;
        public int bit_rate;
        public int sample_rate;
        public string extension;
    }
}