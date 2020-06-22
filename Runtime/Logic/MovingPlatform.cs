using UnityEngine;

namespace traVRsal.SDK
{
    public class MovingPlatform : MonoBehaviour
    {
        public enum Trigger
        {
            MANUAL, AUTOMATIC
        }

        public enum State
        {
            HOME, MOVING_TO_TARGET, TARGET, MOVING_TO_HOME
        }

        public enum AutoMovement
        {
            NONE, AUTO_RETURN, MOVE_TO_PLAYER
        }

        public enum TargetPositionMode
        {
            AUTOMATIC, MANUAL
        }

        public TargetPositionMode targetPositionMode = TargetPositionMode.AUTOMATIC;
        public Vector3 targetPosition;
        public float duration = 2f;
        public float physicsMultiplier = 1f;
        public float playerCheckInterval = 2f;
        public Trigger trigger = Trigger.MANUAL;
        public AutoMovement autoMovement = AutoMovement.NONE;
    }
}