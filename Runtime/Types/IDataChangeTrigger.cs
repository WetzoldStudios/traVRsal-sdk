namespace traVRsal.SDK
{
    public interface IDataChangeTrigger
    {
        void TriggerDataUpdates();

        object GetDataValue();
    }
}