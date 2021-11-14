using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Comparisons/Not Equal (String)")]
    public class NotEqualStringNode : Node
    {
        [Input] public string a;
        [Input] public string b;
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