using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Set Bool Variable")]
    [NodeTint(hex: "#B04040")]
    public class SetBoolVariableNode : Node
    {
        [Input] public bool call;
        [Input] public string varName;
        [Input] public bool value;
        [Output(connectionType: ConnectionType.Override)] public bool Done;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "varName":
                    return varName;

                case "value":
                    return value;


            }
            return null;
        }
    }
}