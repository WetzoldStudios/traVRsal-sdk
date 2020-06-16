using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/Merge")]
    [NodeTint(hex: "#7C52A5")]
    public class MergeNode : Node
    {
        [Input] public bool One;
        [Input] public bool Two;
        [Input] public bool Three;
        [Input] public bool Four;
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

                case "Three":
                    return Three;

                case "Four":
                    return Four;


            }
            return null;
        }
    }
}