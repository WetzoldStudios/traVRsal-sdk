using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Cooldown")]
    [NodeTint(hex: "#7C52A5")]
    public class CooldownNode : Node
    {
        [Input] public bool call;
        [Input] public bool Cancel;
        [Input] public float time;
        [Output(connectionType: ConnectionType.Override)] public bool Start;
        [Output(connectionType: ConnectionType.Override)] public bool Update;
        [Output(connectionType: ConnectionType.Override)] public bool Finish;
        [Output] public float remaining;
        [Output] public float normalized;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "Cancel":
                    return Cancel;

                case "time":
                    return time;


            }
            return null;
        }
    }
}