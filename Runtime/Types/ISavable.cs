namespace traVRsal.SDK
{
    public interface ISavable
    {
        string GetPersistedState();
        void LoadPersistedState(string state);
    }
}