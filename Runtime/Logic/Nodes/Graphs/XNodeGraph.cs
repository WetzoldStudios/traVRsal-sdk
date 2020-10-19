using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    public class XNodeGraph : MonoBehaviour
    {
        public NodeGraph logicGraph;

        private void Start()
        {
        }

        public override string ToString()
        {
            return $"XNode Logic Graph ({logicGraph}))";
        }
    }
}