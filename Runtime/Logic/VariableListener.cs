using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
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

        [Header("Configuration")] public string variable;
        public bool invert;

        [Header("Action")] public Action action = Action.Assignments_Only;
        public GameObject targetObject;
        public Behaviour component;

        [Header("Static Assignments")] public List<Behaviour> enabledComponents;
        public List<Behaviour> disabledComponents;
        public List<Collider> enabledColliders;
        public List<Collider> disabledColliders;
        public List<GameObject> enabledObjects;
        public List<GameObject> disabledObjects;
    }
}