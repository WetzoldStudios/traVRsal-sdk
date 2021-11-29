using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On Int Variable Change")]
    [NodeTint(hex: "#B04040")]
    public class OnIntVariableChangeNode : Node
    {
        [Input] public string varName;
        [Output(connectionType: ConnectionType.Override)] public bool onChange;
        [Output] public int intValue;

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