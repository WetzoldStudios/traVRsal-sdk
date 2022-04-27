using System;
using System.Security;

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
            key = speaker;
            this.backend = backend;
            this.speaker = speaker;
            this.voice = voice;
        }

        private string GetContent(string phrase)
        {
            // check for inline adjustments
            // TODO: deprecate, change to separate voice definition
            string[] arr = StringExt.CleanSplit(phrase, PREFIX_SEP);
            if (arr.Length > 1)
            {
                phrase = arr[arr.Length - 1];
            }

            return phrase;
        }

        public string GetRawText(string phrase)
        {
            // TODO: filter out tags etc for clean subtitles
            return GetContent(phrase);
        }

        public string GetSSML(string phrase)
        {
            switch (backend)
            {
                case TTSBackend.Replica:
                    // more complex tags will lead to malformed URL exceptions as will an empty prosody
                    if (!string.IsNullOrWhiteSpace(pitch) || !string.IsNullOrWhiteSpace(speed))
                    {
                        return "<speak>" +
                               "<prosody" + (!string.IsNullOrWhiteSpace(pitch) ? " pitch=\"" + pitch + "\"" : "")
                               + (!string.IsNullOrWhiteSpace(speed) ? " rate=\"" + speed + "\"" : "") + ">" +
                               SecurityElement.Escape(GetContent(phrase)) +
                               "</prosody></speak>";
                    }
                    return "<speak>" + SecurityElement.Escape(GetContent(phrase)) + "</speak>";

                default:
                    return "<speak version=\"1.0\" xmlns=\"https://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"https://www.w3.org/2001/mstts\" " +
                           "xml:lang=\"" + language + "\">" +
                           "<voice name=\"" + voice + "\">" +
                           "<mstts:express-as type=\"" + mood + "\">" +
                           "<prosody pitch=\"" + pitch + "\" " + (!string.IsNullOrEmpty(speed) ? "rate=\"" + speed + "\"" : "") + ">" +
                           SecurityElement.Escape(GetContent(phrase)) +
                           "</prosody></mstts:express-as></voice></speak>";
            }
        }

        private string GetCompoundKey()
        {
            return backend + "|" + voice + "|" + language + "|" + mood + "|" + speed + "|" + pitch;
        }

        public string GetHashedFileName(string phrase)
        {
            return (GetCompoundKey() + "@" + phrase).GetHashString();
        }

        public override string ToString()
        {
            return $"Voice Spec '{key}' ({speaker}, {backend})";
        }
    }
}