using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Comparisons/Not Equal (Int)")]
    public class NotEqualIntNode : Node
    {
        [Input] public int a;
        [Input] public int b;
        [Output] public bool result;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "a":
                    return a;

                case "b":
                    return b;


            }
            return null;
        }
    }
}