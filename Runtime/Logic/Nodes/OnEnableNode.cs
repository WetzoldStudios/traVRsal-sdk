using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On Enable")]
    [NodeTint(hex: "#B04040")]
    public class OnEnableNode : Node
    {
        [Output(connectionType: ConnectionType.Override)] public bool onEnable;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            return null;
        }
    }
}