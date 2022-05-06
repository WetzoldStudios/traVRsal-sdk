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
            WaitForZone = 2,
            WaitForVariable = 3,
            SetVariable = 4,
            IncVariable = 5,
            DecVariable = 6,
            Empty = 99
        }

        public LineType type = LineType.Empty;
        public string raw;
        public int lineNr;
        public string param;
        public string value;

        public string speaker;
        public string filePath;

        public float duration;

        public StoryAction(string line, int lineNr)
        {
            this.lineNr = lineNr;
            if (string.IsNullOrWhiteSpace(line)) return;
            line = line.Trim();
            raw = line;

            if (line.StartsWith("//")) return;
            if (line.StartsWith(CODE_START))
            {
                line = line.Replace("[", "").Replace("]", "");
                string[] arr = line.Split(':').Select(s => s.Trim()).ToArray();
                if (arr.Length >= 2) param = arr[1];
                if (arr.Length >= 3) value = arr[2];
                switch (arr[0].ToLower())
                {
                    case "pause":
                        type = LineType.Pause;
                        if (!float.TryParse(arr[1], NumberStyles.Number, NumberFormatInfo.InvariantInfo, out duration))
                        {
                            EDebug.LogError($"Story contains invalid Pause value on line {lineNr}: {line}");
                        }
                        break;

                    case "waitforzone":
                        type = LineType.WaitForZone;
                        break;

                    case "waitforvariable":
                        type = LineType.WaitForVariable;
                        break;

                    case "setvariable":
                        type = LineType.SetVariable;
                        if (string.IsNullOrWhiteSpace(value)) value = "true";
                        break;

                    case "incvariable":
                        type = LineType.IncVariable;
                        break;

                    case "decvariable":
                        type = LineType.IncVariable;
                        break;

                    default:
                        EDebug.LogError($"Story contains invalid command on line {lineNr}: {arr[0]}");
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
                param = line;
            }
        }

        public override string ToString()
        {
            return $"{type} ({param})";
        }
    }
}