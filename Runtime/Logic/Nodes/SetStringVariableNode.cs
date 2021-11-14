using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Set String Variable")]
    [NodeTint(hex: "#B04040")]
    public class SetStringVariableNode : Node
    {
        [Input] public bool call;
        [Input] public string varName;
        [Input] public string value;
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