using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Get String Variable")]
    [NodeTint(hex: "#B04040")]
    public class GetStringVariableNode : Node
    {
        [Input] public string varName;
        [Output] public string value;

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