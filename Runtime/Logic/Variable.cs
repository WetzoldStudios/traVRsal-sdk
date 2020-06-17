using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Variable
    {
        public enum Behaviour
        {
            UNRESTRICTED, CHANGE_HIGHER
        }

        [Header("Configuration")]
        public string key;
        public string value = "0";
        public Behaviour behaviour = Behaviour.UNRESTRICTED;
        public string imageFolder;
        public int targetCount = 0;

        [Header("Runtime")]
        public bool runtimeCreated = false;
        public bool isComboPart = false;
        public List<string> targetOrder = new List<string>();
        public List<string> currentOrder = new List<string>();
        public List<IVariableListener> listeners = new List<IVariableListener>();
        public int currentAutoIndex = 0;

        [NonSerialized]
        public Variable parent;
        [NonSerialized]
        public List<Variable> children;

        public Variable(string key)
        {
            this.key = key;
        }

        public override string ToString()
        {
            return $"Variable {key} ({value}, {behaviour})";
        }
    }
}