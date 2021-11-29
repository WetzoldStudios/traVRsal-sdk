using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Events/On String Variable Change")]
    [NodeTint(hex: "#B04040")]
    public class OnStringVariableChangeNode : Node
    {
        [Input] public string varName;
        [Output(connectionType: ConnectionType.Override)] public bool onChange;
        [Output] public string stringValue;

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