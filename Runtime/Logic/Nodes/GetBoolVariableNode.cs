using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Get Bool Variable")]
    [NodeTint(hex: "#B04040")]
    public class GetBoolVariableNode : Node
    {
        [Input] public string varName;
        [Output] public bool value;

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