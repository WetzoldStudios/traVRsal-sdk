using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Trigger Reactor")]
    public class TriggerReactor : ExecutorConfig
    {
        [Range(0, 5)] public int variableChannel;

        [Tooltip("Tag of object that should be reacted on by the trigger. Separate multiple by comma.")]
        public string source = SDKUtil.PLAYER_HEAD_TAG;

        [Tooltip("Name of audio file under Audio/Effects")]
        public string audioEffect;

        public bool audioEffectOnlyOnce = true;
        [Tooltip("Text to be spoken by TTS")] public string speak;
        public bool speakOnlyOnce = true;

        public override string ToString()
        {
            return "Trigger Reactor";
        }
    }
}