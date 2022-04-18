using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Trigger Reactor")]
    public class TriggerReactor : ExecutorConfig
    {
        [Range(0, 5)] public int variableChannel;

        [Tooltip("Tag of object that should be reacted on by the trigger. Separate multiple by comma.")]
        public string source = SDKUtil.PLAYER_HEAD_TAG;

        [Tooltip("Delay while object needs to remain in trigger before trigger reacts. Will fire OnTriggerValid once time has passed.")]
        public float delay;

        [Tooltip("Delay until trigger will execute again")]
        public float cooldown;

        [Tooltip("Name of variable that must be true before validation can succeed")]
        public string validationCondition;

        [Tooltip("Name of audio file under Audio/Effects")]
        public string audioEffect;

        public bool audioEffectOnlyOnce = true;
        [Tooltip("Text to be spoken by TTS")] public string speak;
        public bool speakOnlyOnce = true;

        [Header("Events")] public UnityEvent onTriggerEnter;

        [Tooltip("Raised if a delay is set and delay has passed")]
        public UnityEvent onActivated;

        [Tooltip("Raised if a delay is set and validation condition is set and true")]
        public UnityEvent onValidated;

        [Tooltip("Raised if a delay is set and validation condition is set but not true")]
        public UnityEvent onNotValidated;

        public UnityEvent onTriggerExit;

        public override string ToString()
        {
            return $"Trigger Reactor ({source})";
        }
    }
}