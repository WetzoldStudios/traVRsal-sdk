using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Get Float Variable")]
    [NodeTint(hex: "#B04040")]
    public class GetFloatVariableNode : Node
    {
        [Input] public string varName;
        [Output] public float value;

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