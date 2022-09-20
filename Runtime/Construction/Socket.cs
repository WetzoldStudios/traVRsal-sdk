using UnityEngine;

namespace traVRsal.SDK
{
    public sealed class Socket : MonoBehaviour
    {
        public string key;

        [Tooltip("Set to true if the socket is inside the play area for additional performance and visibility optimizations. It will inherit the position of the parent object for reference.")]
        public bool cullable = true;

        [Tooltip("Distance attached object needs to move in order to mark socket as free again.")]
        public float breakDistance = 0.5f;

        [Tooltip("Will move the socket vertically depending on height of player.")]
        public bool considerPlayerHeight;

        [HideInInspector] public GameObject item;

        // cache only
        [HideInInspector] public Rigidbody rigidBody;
        [HideInInspector] public bool isKinematic;

        private void Update()
        {
            if (item == null) return;

            // release socket if item moves away far enough
            if (rigidBody != null)
            {
                if (rigidBody.transform.GetDistanceAbs(transform) > breakDistance) RemoveItem();
            }
            else
            {
                if (item.transform.GetDistanceAbs(transform) > breakDistance) RemoveItem();
            }
        }

        public Socket(string key)
        {
            this.key = key;
        }

        public void AddItem(GameObject newItem)
        {
            item = newItem;

            rigidBody = newItem.GetComponentInChildren<Rigidbody>();
            if (rigidBody != null) isKinematic = rigidBody.isKinematic;
        }

        public void RemoveItem()
        {
            item = null;
            rigidBody = null;
        }

        public override string ToString()
        {
            return $"Socket '{key}'";
        }
    }
}