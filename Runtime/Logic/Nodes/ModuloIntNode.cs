using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Calculations/Modulo Int")]
    public class ModuloIntNode : Node
    {
        [Input] public int value;
        [Input] public int mod;
        [Output] public int result;

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