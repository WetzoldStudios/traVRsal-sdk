using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class ReplicaSpeech
    {
        public string uuid;
        public string quality;
        public string txt;
        public string speaker_id;
        public int bit_rate;
        public int sample_rate;
        public string extension;
        public string[] extensions;
        public string url;
        public float duration;

        // public string[] urls;
    }
}