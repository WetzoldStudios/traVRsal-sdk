namespace traVRsal.SDK
{
    public class TriggerReactor : ExecutorConfig
    {
        public string source = SDKUtil.PLAYER_HEAD_TAG;
        public string audioEffect;
        public bool audioEffectOnlyOnce = true;
        public string speak;
        public bool speakOnlyOnce = true;

        public override string ToString()
        {
            return "TriggerReactor";
        }
    }
}