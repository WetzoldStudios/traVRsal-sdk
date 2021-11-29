using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Misc/LogWarning")]
    public class LogWarningNode : Node
    {
        [Input] public bool call;
        [Input] public string a;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "a":
                    return a;


            }
            return null;
        }
    }
}