using System;
using System.Collections;
using UnityEngine;

namespace traVRsal.SDK
{
    public interface ISpawner
    {
        IEnumerator Spawn(string key, BasicEntity newEntity, Action<GameObject> callback = null);

        IEnumerator Spawn(SpawnRule rule, Action<GameObject> callback = null);

        void Destruct(GameObject go, bool partial = false);
    }
}