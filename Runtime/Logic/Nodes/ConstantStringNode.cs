using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Variables/Constant String")]
    [NodeTint(hex: "#B04040")]
    public class ConstantStringNode : Node
    {
        [Input] public string a;
        [Output] public string value;

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