using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class VoiceSpec
    {
        public const char PREFIX_SEP = '|';

        public enum TTSBackend
        {
            Default = 0,
            Google = 1,
            Microsoft = 2,
            Replica = 3
        }

        public TTSBackend backend;
        public string key;
        public string speaker;
        public string voice;

        // backend-dependent
        public string language;
        public string mood;
        public string speed;
        public string pitch;

        public VoiceSpec()
        {
        }

        public VoiceSpec(TTSBackend backend, string speaker, string voice) : this()
        {
            this.backend = backend;
            this.key = speaker;
            this.speaker = speaker;
            this.voice = voice;
        }

        public string GetText(string phrase)
        {
            // check for inline adjustments
            // TODO: decommit, change to separate voice definition
            string[] arr = StringExt.CleanSplit(phrase, PREFIX_SEP);
            if (arr.Length > 1)
            {
                phrase = arr[arr.Length - 1];
            }

            return phrase;
        }

        public override string ToString()
        {
            return $"Voice Spec '{key}' ({speaker}, {backend})";
        }
    }
}