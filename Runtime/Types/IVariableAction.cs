namespace traVRsal.SDK
{
    public interface IVariableAction
    {
        bool ReachActionMin(int variableChannel);
        bool ReachActionMin(string key);

        bool ReachActionMax(int variableChannel);
        bool ReachActionMax(string key);

        void ToggleAction(int variableChannel);
        void ToggleAction(string key);

        void Increase(string key);
        void Decrease(string key);
    }
}