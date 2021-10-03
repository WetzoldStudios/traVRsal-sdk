using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Variable Listener")]
    public class VariableListener : ExecutorConfig
    {
        public enum Action
        {
            Assignments_Only = 6,
            Call_Reactors = 3,
            Activate_Object = 0,
            Activate_Component = 1,
            Activate_Emission = 2,
            GameState_Game = 4,
            GameState_Menu = 5,
            Nothing = 7
        }

        [Header("Configuration")] [Tooltip("Variable to listen to. If left empty will use the variable defined from the outside, e.g. through Tiled.")]
        public string variable;

        [Range(0, 5)] public int variableChannel;

        public bool invert;

        [Header("Action")] public Action action = Action.Assignments_Only;
        public GameObject targetObject;
        public Behaviour component;

        [Header("Static Assignments")] [Obsolete]
        public List<GameObject> enabledObjects;

        [Obsolete] public List<GameObject> disabledObjects;
        [Obsolete] public List<Behaviour> enabledComponents;
        [Obsolete] public List<Behaviour> disabledComponents;
        [Obsolete] public List<Collider> enabledColliders;
        [Obsolete] public List<Collider> disabledColliders;

        [Header("Events")] public UnityEvent onTrue;
        public UnityEvent onFalse;
        public UnityEvent<bool> onChange;
    }
}