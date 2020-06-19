using Newtonsoft.Json;
using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class TextFragment
    {
        public enum Event
        {
            LOADING_DONE, VARIABLE_ON, VARIABLE_OFF, ENTER_ROOM, ENTER_TRANSITION, SEE_OBJECT, HEAR_OBJECT
        }

        public enum Repetition
        {
            ONCE, ALWAYS
        }

        [JsonProperty("event")]
        public Event worldEvent = Event.LOADING_DONE;
        public Repetition repetition = Repetition.ONCE;
        public string key;
        public string text;
        public float proximity = -1;

        public bool played = false;

        public TextFragment() { }

        public TextFragment(Event worldEvent, string text) : this()
        {
            this.worldEvent = worldEvent;
            this.text = text;
        }

        public TextFragment(Event worldEvent, string key, string text) : this(worldEvent, text)
        {
            this.key = key;
        }

        public override string ToString()
        {
            return $"TextFragment {worldEvent} ({key}, {repetition})";
        }
    }
}