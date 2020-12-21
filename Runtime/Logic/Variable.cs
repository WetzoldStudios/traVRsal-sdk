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
            Unrestricted,
            Change_Higher
        }

        [Header("Configuration")] public string key;
        public string value = "0";
        public Behaviour behaviour = Behaviour.Unrestricted;
        public string imageFolder;
        public int targetCount;

        [Header("Runtime")] public bool runtimeCreated;
        public bool isComboPart;
        public List<string> targetOrder = new List<string>();
        public List<string> currentOrder = new List<string>();
        public List<IVariableListener> listeners = new List<IVariableListener>();
        public int currentAutoIndex;

        [NonSerialized] public Variable parent;
        [NonSerialized] public List<Variable> children;
        [NonSerialized] public int changeFrame;
        [NonSerialized] public bool everChanged;

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