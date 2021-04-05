namespace traVRsal.SDK
{
    public class OpenBrowser : ExecutorConfig
    {
        public enum UserAction
        {
            Touch = 0,
            Trigger = 1,
            UIButton = 2
        }

        public UserAction action;
 
        public DataBinding urlBinding;
        public string directUrl;
    }
}