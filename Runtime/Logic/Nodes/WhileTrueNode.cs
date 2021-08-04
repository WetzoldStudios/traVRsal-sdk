using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/While True")]
    [NodeTint(hex: "#7C52A5")]
    public class WhileTrueNode : Node
    {
        [Input] public bool call;
        [Input] public bool condition;
        [Output(connectionType: ConnectionType.Override)] public bool Do;
        [Output(connectionType: ConnectionType.Override)] public bool Done;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "condition":
                    return condition;


            }
            return null;
        }
    }
}