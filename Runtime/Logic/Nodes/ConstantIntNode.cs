using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Constant Int")]
    [NodeTint(hex: "#B04040")]
    public class ConstantIntNode : Node
    {
        [Input] public int a;
        [Output] public int value;

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