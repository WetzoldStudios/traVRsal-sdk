using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Constant Bool")]
    [NodeTint(hex: "#B04040")]
    public class ConstantBoolNode : Node
    {
        [Input] public bool a;
        [Output] public bool value;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "a":
                    return a;


            }
            return null;
        }
    }
}