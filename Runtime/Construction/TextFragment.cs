using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace traVRsal.SDK
{
    [Serializable]
    public class TextFragment
    {
        public enum Event
        {
            Loading_Done,
            Variable_On,
            Variable_Off,
            Enter_Zone,
            Enter_Transition,
            See_Object,
            Hear_Object
        }

        public enum Repetition
        {
            Once,
            Always
        }

        [JsonProperty("event")] public Event worldEvent = Event.Loading_Done;
        public Repetition repetition = Repetition.Once;
        public string key;
        public string text;
        [DefaultValue(-1f)] public float proximity = -1;

        public bool played;

        public TextFragment()
        {
        }

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
            return $"Text Fragment {worldEvent} ({key}, {repetition})";
        }
    }
}