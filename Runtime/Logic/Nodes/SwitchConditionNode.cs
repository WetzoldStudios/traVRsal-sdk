using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Switch Condition")]
    [NodeTint(hex: "#7C52A5")]
    public class SwitchConditionNode : Node
    {
        [Input] public bool call;
        [Input] public bool condition;
        [Output(connectionType: ConnectionType.Override)] public bool ifTrue;
        [Output(connectionType: ConnectionType.Override)] public bool ifFalse;

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