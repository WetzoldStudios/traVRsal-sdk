using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Story Player")]
    public class StoryPlayer : MonoBehaviour
    {
        public TextAsset file;

        [Header("Events")] public UnityEvent onCompletion;

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            
        }
    }
}