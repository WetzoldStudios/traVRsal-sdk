using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Strings/Substring")]
    public class SubstringNode : Node
    {
        [Input] public string a;
        [Input] public int b;
        [Input] public int c;
        [Output] public string value;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "a":
                    return a;

                case "b":
                    return b;

                case "c":
                    return c;


            }
            return null;
        }
    }
}