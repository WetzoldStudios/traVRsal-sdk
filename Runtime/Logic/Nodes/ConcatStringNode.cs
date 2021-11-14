using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Strings/Concat String")]
    public class ConcatStringNode : Node
    {
        [Input] public string a;
        [Input] public string b;
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


            }
            return null;
        }
    }
}