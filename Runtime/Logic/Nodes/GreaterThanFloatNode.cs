using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Comparisons/Greater Than (Float)")]
    public class GreaterThanFloatNode : Node
    {
        [Input] public float a;
        [Input] public float b;
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