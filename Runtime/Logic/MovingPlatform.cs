using UnityEngine;

namespace traVRsal.SDK
{
    public class MovingPlatform : ExecutorConfig
    {
        public enum Trigger
        {
            Manual,
            Automatic
        }

        public enum State
        {
            Home,
            Moving_To_Target,
            Target,
            Moving_To_Home
        }

        public enum AutoMovement
        {
            None,
            Auto_Return,
            Move_To_Player
        }

        public enum TargetPositionMode
        {
            Automatic,
            Manual,
            Location
        }

        public TargetPositionMode targetPositionMode = TargetPositionMode.Automatic;
        public Vector3 targetPosition;
        public string locationId;
        public float duration = 2f;
        public float physicsMultiplier = 1f;
        public float playerCheckInterval = 2f;
        public Trigger trigger = Trigger.Manual;
        public AutoMovement autoMovement = AutoMovement.None;
    }
}