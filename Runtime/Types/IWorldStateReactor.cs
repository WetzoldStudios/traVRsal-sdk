using UnityEngine;

namespace traVRsal.SDK
{
    public interface IWorldStateReactor
    {
        void FinishedLoading(Vector3 tileSizes);
    }
}