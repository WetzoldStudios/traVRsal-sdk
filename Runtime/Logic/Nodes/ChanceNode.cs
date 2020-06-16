using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Chance")]
    [NodeTint(hex: "#7C52A5")]
    public class ChanceNode : Node
    {
        [Input] public bool call;
        [Input] public float percentage;
        [Input] public float min;
        [Input] public float max;
        [Output(connectionType: ConnectionType.Override)] public bool Success;
        [Output(connectionType: ConnectionType.Override)] public bool Failure;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "percentage":
                    return percentage;

                case "min":
                    return min;

                case "max":
                    return max;


            }
            return null;
        }
    }
}