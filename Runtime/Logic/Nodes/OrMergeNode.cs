using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/OrMerge")]
    [NodeTint(hex: "#7C52A5")]
    public class OrMergeNode : Node
    {
        [Input] public bool One;
        [Input] public bool Two;
        [Output(connectionType: ConnectionType.Override)] public bool Merged;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "One":
                    return One;

                case "Two":
                    return Two;


            }
            return null;
        }
    }
}