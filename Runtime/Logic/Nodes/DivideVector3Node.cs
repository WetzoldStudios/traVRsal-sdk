using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Calculations/Divide Vector3")]
    public class DivideVector3Node : Node
    {
        [Input] public Vector3 a;
        [Input] public Vector3 b;
        [Output] public Vector3 result;

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