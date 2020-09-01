using UnityEngine;

namespace traVRsal.SDK
{
    public class Socket : MonoBehaviour
    {
        public string key;
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