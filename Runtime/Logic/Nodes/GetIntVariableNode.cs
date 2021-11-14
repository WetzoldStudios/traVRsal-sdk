using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Get Int Variable")]
    [NodeTint(hex: "#B04040")]
    public class GetIntVariableNode : Node
    {
        [Input] public string varName;
        [Output] public int value;

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