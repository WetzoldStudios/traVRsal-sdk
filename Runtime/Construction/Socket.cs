using UnityEngine;

namespace traVRsal.SDK
{
    public class Socket : MonoBehaviour
    {
        public string key;

        [HideInInspector]
        public GameObject item;
        [HideInInspector]
        public Rigidbody rigidBody; // cache only

        public Socket() { }

        public Socket(string key) : this()
        {
            this.key = key;
        }

        public void AddItem(GameObject item)
        {
            this.item = item;

            rigidBody = item.GetComponent<Rigidbody>();
        }

        public void RemoveItem()
        {
            item = null;
            rigidBody = null;
        }

        public override string ToString()
        {
            return $"Socket ({key}, {item})";
        }
    }
}