using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Constant Float")]
    [NodeTint(hex: "#B04040")]
    public class ConstantFloatNode : Node
    {
        [Input] public float a;
        [Output] public float value;

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