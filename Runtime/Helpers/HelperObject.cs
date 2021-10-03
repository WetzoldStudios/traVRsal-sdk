using UnityEngine;
using UnityEngine.SceneManagement;

namespace traVRsal.SDK
{
    public class HelperObject : MonoBehaviour
    {
        public enum RemovalType
        {
            OnRun = 0,
            IfNotOnlyScene = 1
        }

        public RemovalType removalType = RemovalType.IfNotOnlyScene;

        private void Start()
        {
            switch (removalType)
            {
                case RemovalType.IfNotOnlyScene:
                    if (SceneManager.sceneCount == 1) return;
                    break;
            }
            Destroy(gameObject);
        }
    }
}