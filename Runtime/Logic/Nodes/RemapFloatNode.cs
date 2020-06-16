using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Calculations/Remap Float")]
    public class RemapFloatNode : Node
    {
        [Input] public float current;
        [Input] public float inputMin;
        [Input] public float inputMax;
        [Input] public float outputMin;
        [Input] public float outputMax;
        [Output] public float newValue;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "current":
                    return current;

                case "inputMin":
                    return inputMin;

                case "inputMax":
                    return inputMax;

                case "outputMin":
                    return outputMin;

                case "outputMax":
                    return outputMax;


            }
            return null;
        }
    }
}