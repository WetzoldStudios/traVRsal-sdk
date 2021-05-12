using UnityEngine;

namespace traVRsal.SDK
{
    public interface IWorldStateReactor
    {
        void ZoneChange(Zone zone, bool isCurrent);

        void FinishedLoading(Vector3 tileSizes, bool instantEnablement = false);
    }
}