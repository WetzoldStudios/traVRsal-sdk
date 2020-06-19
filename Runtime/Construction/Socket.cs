using UnityEngine;

namespace traVRsal.SDK
{
    public class Socket : MonoBehaviour
    {
        public enum SocketType
        {
            INVENTORY, ENEMY_BOTTOM, ENEMY_TOP, ENEMY_SIDE
        }

        public SocketType type = SocketType.INVENTORY;
        [HideInInspector]
        public GameObject item;
        [HideInInspector]
        public Rigidbody rigidBody; // cache only

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
            return $"{type} socket ({item})";
        }
    }
}