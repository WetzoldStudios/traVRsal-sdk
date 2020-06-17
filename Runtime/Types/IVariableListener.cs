namespace traVRsal.SDK
{
    public interface IVariableListener
    {
        void Init();

        void VariableChange(Variable variable);
    }
}