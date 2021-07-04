namespace traVRsal.SDK
{
    public interface IVariableAction
    {
        void ReachActionMin(int variableChannel);

        void ReachActionMax(int variableChannel);

        void ToggleAction(int variableChannel);
    }
}