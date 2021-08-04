using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Flow/For Loop")]
    [NodeTint(hex: "#7C52A5")]
    public class ForLoopNode : Node
    {
        [Input] public bool call;
        [Input] public bool Break;
        [Input] public int first;
        [Input] public int last;
        [Input] public int step;
        [Output] public int index;
        [Output(connectionType: ConnectionType.Override)] public bool Do;
        [Output(connectionType: ConnectionType.Override)] public bool Done;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "Break":
                    return Break;

                case "first":
                    return first;

                case "last":
                    return last;

                case "step":
                    return step;


            }
            return null;
        }
    }
}