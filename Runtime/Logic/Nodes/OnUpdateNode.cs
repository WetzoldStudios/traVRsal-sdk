using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On Update")]
    [NodeTint(hex: "#B04040")]
    public class OnUpdateNode : Node
    {
        [Output(connectionType: ConnectionType.Override)] public bool onUpdate;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            return null;
        }
    }
}