using System;
using System.Globalization;
using System.Linq;

namespace traVRsal.SDK
{
    [Serializable]
    public class StoryAction
    {
        public const string CODE_START = "[";

        public enum LineType
        {
            Speech = 0,
            Pause = 1,
            Empty = 99
        }

        public LineType type = LineType.Empty;
        public string raw;
        public string content;

        public string speaker;
        public string filePath;

        public float duration;

        public StoryAction(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;
            line = line.Trim();
            raw = line;

            if (line.StartsWith(CODE_START))
            {
                line = line.Replace("[", "").Replace("]", "");
                string[] arr = line.Split(':').Select(s => s.Trim()).ToArray();
                switch (arr[0].ToLower())
                {
                    case "pause":
                        type = LineType.Pause;
                        if (!float.TryParse(arr[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out duration))
                        {
                            EDebug.LogError($"Story contains invalid Pause value: {line}");
                        }
                        break;
                }
            }
            else
            {
                type = LineType.Speech;
                int speakerPos = line.IndexOf(':');
                if (speakerPos > 0)
                {
                    speaker = line.Substring(0, speakerPos).Trim();
                    line = line.Substring(speakerPos + 1).Trim();
                }
                content = line;
            }
        }

        public override string ToString()
        {
            return "Story Action";
        }
    }
}