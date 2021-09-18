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
            LoadingDone,
            VariableOn,
            VariableOff,
            EnterZone,
            EnterTransition,
            SeeObject,
            HearObject
        }

        public enum Repetition
        {
            Once,
            Always
        }

        [JsonProperty("event")] public Event worldEvent = Event.LoadingDone;
        public Repetition repetition = Repetition.Once;
        public string key;
        public string audioEffect;
        public string text;
        [DefaultValue(-1f)] public float proximity = -1;

        public bool played;

        public TextFragment()
        {
        }

        public TextFragment(Event worldEvent) : this()
        {
            this.worldEvent = worldEvent;
        }

        public TextFragment(Event worldEvent, string text) : this(worldEvent)
        {
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