using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Do Once")]
    [NodeTint(hex: "#7C52A5")]
    public class DoOnceNode : Node
    {
        [Input] public bool call;
        [Input] public bool Reset;
        [Output(connectionType: ConnectionType.Override)] public bool outFlow;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "Reset":
                    return Reset;


            }
            return null;
        }
    }
}