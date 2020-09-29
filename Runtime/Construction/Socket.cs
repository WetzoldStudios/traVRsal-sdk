using UnityEngine;

namespace traVRsal.SDK
{
    public class Socket : MonoBehaviour
    {
        public string key;
        [Tooltip("Set to true if the socket is inside the play area for additional performance and visibility optimizations. It will inherit the position of the parent object for reference.")]
        public bool cullable = true;

        [HideInInspector]
        public GameObject item;

        // cache only
        [HideInInspector]
        public Rigidbody rigidBody;
        [HideInInspector]
        public bool isKinematic = false;

        public Socket() { }

        public Socket(string key) : this()
        {
            this.key = key;
        }

        public void AddItem(GameObject item)
        {
            this.item = item;

            rigidBody = item.GetComponent<Rigidbody>();
            if (rigidBody != null) isKinematic = rigidBody.isKinematic;
        }

        public void RemoveItem()
        {
            item = null;
            rigidBody = null;
        }

        public override string ToString()
        {
            return $"Socket ({key})";
        }
    }
}