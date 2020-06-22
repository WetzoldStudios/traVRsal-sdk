using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Wait While")]
    [NodeTint(hex: "#7C52A5")]
    public class WaitWhileNode : Node
    {
        [Input] public bool call;
        [Input] public bool condition;
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