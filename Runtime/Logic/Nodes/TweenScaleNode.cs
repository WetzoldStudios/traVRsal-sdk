using UnityEngine;
using XNode;

namespace traVRsal.SDK
{
    [CreateNodeMenu(menuName: "Objects/Tween Scale")]
    public class TweenScaleNode : Node
    {
        [Input] public bool call;
        [Input] public Transform transform;
        [Input] public Vector3 scale;
        [Input] public float duration;
        [Input] public bool relative;

        public override object GetValue(NodePort port)
        {
            if (port.IsOutput) return null;

            switch (port.fieldName)
            {
                case "call":
                    return call;

                case "transform":
                    return transform;

                case "scale":
                    return scale;

                case "duration":
                    return duration;

                case "relative":
                    return relative;


            }
            return null;
        }
    }
}