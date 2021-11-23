using System.Collections;

namespace traVRsal.SDK
{
    public interface ISpawner
    {
        IEnumerator Spawn(string key);

        IEnumerator Spawn(BasicSpawnRule rule);
    }
}