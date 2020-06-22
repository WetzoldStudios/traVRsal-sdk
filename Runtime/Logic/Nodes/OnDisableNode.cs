using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On Disable")]
    [NodeTint(hex: "#B04040")]
    public class OnDisableNode : Node
    {
        [Output(connectionType: ConnectionType.Override)] public bool onDisable;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            return null;
        }
    }
}