using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On Bool Variable Change")]
    [NodeTint(hex: "#B04040")]
    public class OnBoolVariableChangeNode : Node
    {
        [Input] public string varName;
        [Output(connectionType: ConnectionType.Override)] public bool onChange;
        [Output] public bool boolValue;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "varName":
                    return varName;


            }
            return null;
        }
    }
}