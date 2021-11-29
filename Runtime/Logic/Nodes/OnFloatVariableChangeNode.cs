using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On Float Variable Change")]
    [NodeTint(hex: "#B04040")]
    public class OnFloatVariableChangeNode : Node
    {
        [Input] public string varName;
        [Output(connectionType: ConnectionType.Override)] public bool onChange;
        [Output] public float floatValue;

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