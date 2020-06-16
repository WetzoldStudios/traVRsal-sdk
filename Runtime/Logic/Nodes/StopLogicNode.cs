using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Stop Logic")]
    [NodeTint(hex: "#7C52A5")]
    public class StopLogicNode : Node
    {
        [Input] public bool call;
        [Input] public bool success;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "success":
                    return success;


            }
            return null;
        }
    }
}