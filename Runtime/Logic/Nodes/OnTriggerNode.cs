using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On Trigger")]
    [NodeTint(hex: "#B04040")]
    public class OnTriggerNode : Node
    {
        [Output(connectionType: ConnectionType.Override)] public bool onEnter;
        [Output(connectionType: ConnectionType.Override)] public bool onStay;
        [Output(connectionType: ConnectionType.Override)] public bool onExit;
        [Output] public Collider collider;
        [Output] public GameObject other;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            return null;
        }
    }
}