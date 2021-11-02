using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Enemy Behavior")]
    public class EnemyBehavior : ExecutorConfig
    {
        public enum EnemyState
        {
            Scan,
            Investigate,
            Engage
        }

        public enum Movement
        {
            None,
            Random
        }

        [Header("General")] public EnemyState state = EnemyState.Scan;
        public float investigateToEngageDelay = 0.5f;
        public float investigateToScanDelay = 3f;
        public float engageToScanDelay = 3f;
        public BulletHellManager scanBehavior;
        public BulletHellManager investigateBehavior;
        public BulletHellManager engageBehavior;

        [Header("Tracking")] public bool trackPlayer = true;
        public Transform tracker;
        public float trackSpeed = 1f;

        [Header("Attack")] public bool proximityDamage;
        public float proximity = 0.5f;

        [Header("Movement")] public Movement movement = Movement.None;
        public float minMoveChangeDelay = 2f;
        public float maxMoveChangeDelay = 5f;
        public Vector2 moveAreaStart = Vector2.zero;
        public Vector2 moveAreaEnd = Vector2.zero;
        public float wanderRadius = 5f;
        public bool snapToGrid = true;
        public bool reportMovement = true;
        public AudioSource walkSound;

        [Tooltip("Name of bool parameter defining if agent is moving or not")]
        public string animParamMove = "move";

        [Tooltip("Name of float parameter specifying the velocity in x direction")]
        public string animParamVelocityX = "velx";

        [Tooltip("Name of float parameter specifying the velocity in z direction")]
        public string animParamVelocityZ = "velz";
    }
}