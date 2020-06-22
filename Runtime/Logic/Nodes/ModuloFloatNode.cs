using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Calculations/Modulo Float")]
    public class ModuloFloatNode : Node
    {
        [Input] public float value;
        [Input] public float mod;
        [Output] public float result;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "value":
                    return value;

                case "mod":
                    return mod;


            }
            return null;
        }
    }
}