using System;
using System.Collections.Generic;
using System.Linq;

namespace traVRsal.SDK
{
    [Serializable]
    public class StoryScript
    {
        public List<StoryAction> actions;

        private char[] _delim = {'\r', '\n'};
        private string[] _rawLines;

        public StoryScript(string text)
        {
            _rawLines = text.Split(_delim, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("//"))
                .ToArray();

            actions = new List<StoryAction>();
            _rawLines.ForEach(line => actions.Add(new StoryAction(line)));
        }

        public int GetActionCount() => _rawLines.Length;

        public override string ToString()
        {
            return "Story Script";
        }
    }
}