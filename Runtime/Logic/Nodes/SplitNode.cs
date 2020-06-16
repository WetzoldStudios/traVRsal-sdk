using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Split")]
    [NodeTint(hex: "#7C52A5")]
    public class SplitNode : Node
    {
        [Input] public bool call;
        [Output(connectionType: ConnectionType.Override)] public bool one;
        [Output(connectionType: ConnectionType.Override)] public bool two;
        [Output(connectionType: ConnectionType.Override)] public bool three;
        [Output(connectionType: ConnectionType.Override)] public bool four;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;


            }
            return null;
        }
    }
}