namespace traVRsal.SDK
{
    public interface IVariableAction
    {
        void ReachActionMin(int variableChannel);
        void ReachActionMin(string key);

        void ReachActionMax(int variableChannel);
        void ReachActionMax(string key);

        void ToggleAction(int variableChannel);
        void ToggleAction(string key);
    }
}