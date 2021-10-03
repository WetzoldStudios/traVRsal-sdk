namespace traVRsal.SDK
{
    public interface IVariableListener
    {
        void Init();

        void DeInit();

        void VariableChange(Variable variable, bool initialCall = false);
    }
}