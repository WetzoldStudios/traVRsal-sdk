using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace traVRsal.SDK
{
    [Serializable]
    public class StoryScript
    {
        public List<StoryAction> actions;

        private string[] _rawLines;

        public StoryScript(string text)
        {
            _rawLines = Regex.Split(text, "\r\n|\r|\n")
                .Select(line => line.Trim())
                .ToArray();

            actions = new List<StoryAction>();
            for (int i = 0; i < _rawLines.Length; i++)
            {
                StoryAction action = new StoryAction(_rawLines[i], i + 1);
                if (action.type == StoryAction.LineType.Empty) continue;

                actions.Add(action);
            }
        }

        public int GetActionCount() => _rawLines.Length;

        public override string ToString()
        {
            return "Story Script";
        }
    }
}