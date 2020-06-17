using UnityEngine;

namespace traVRsal.SDK
{
    public class VariableListener : MonoBehaviour
    {
        public enum Action
        {
            ACTIVE_OBJECT, ACTIVE_COMPONENT, ACTIVE_EMISSION, CALL_REACTORS, GAMESTATE_GAME, GAMESTATE_MENU
        }
        public string variable;
        public Action action = Action.ACTIVE_OBJECT;
        public GameObject targetObject;
        public Behaviour component;
        public bool invert = false;
    }
}