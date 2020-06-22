using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Wait")]
    [NodeTint(hex: "#7C52A5")]
    public class WaitNode : Node
    {
        [Input] public bool call;
        [Input] public float time;
        [Output] public float timeLeft;
        [Output] public float timeLeftNorm;
        [Output(connectionType: ConnectionType.Override)] public bool Done;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "time":
                    return time;


            }
            return null;
        }
    }
}