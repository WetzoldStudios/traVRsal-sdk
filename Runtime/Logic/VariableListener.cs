using UnityEngine;

namespace traVRsal.SDK
{
    public class VariableListener : ExecutorConfig
    {
        public enum Action
        {
            Activate_Object, Activate_Component, Activate_Emission, Call_Reactors, GameState_Game, GameState_Menu
        }
        public string variable;
        public Action action = Action.Activate_Object;
        public GameObject targetObject;
        public Behaviour component;
        public bool invert = false;
    }
}