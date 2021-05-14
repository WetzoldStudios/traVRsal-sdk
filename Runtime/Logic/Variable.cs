using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Variable : ISavable
    {
        public enum Behaviour
        {
            Unrestricted,
            Change_Higher
        }

        [Header("Configuration")] public string key;
        public object value = false;
        public Behaviour behaviour = Behaviour.Unrestricted;
        public string imageFolder;
        [DefaultValue(3)] public int targetCount = 3;

        [Header("Runtime")] public bool runtimeCreated;
        public bool isComboPart;
        public List<string> targetOrder = new List<string>();
        public List<string> currentOrder = new List<string>();
        public int currentAutoIndex;

        [NonSerialized] public List<IVariableListener> listeners = new List<IVariableListener>();
        [NonSerialized] public Variable parent;
        [NonSerialized] public List<Variable> children;
        [NonSerialized] public List<Variable> affects;
        [NonSerialized] public int changeFrame;
        [NonSerialized] public bool everChanged;

        public Variable(string key)
        {
            this.key = key;
        }

        public void Merge(Variable copyFrom)
        {
            value = copyFrom.value;
            behaviour = copyFrom.behaviour;
            imageFolder = copyFrom.imageFolder;
            targetCount = copyFrom.targetCount;
            targetOrder = copyFrom.targetOrder;
            currentOrder = copyFrom.currentOrder;
            everChanged = copyFrom.everChanged;

            // ensure we are always using floats when using decimals for compatibility (e.g. FlowCanvas sync)
            if (value is double) value = Convert.ChangeType(value, typeof(float));
        }

        public string GetPersistedState()
        {
            return SDKUtil.SerializeObject(this);
        }

        public void LoadPersistedState(string state)
        {
            Variable v = JsonConvert.DeserializeObject<Variable>(state);
            Merge(v);
        }

        protected bool Equals(Variable other)
        {
            return key == other.key && value == other.value && behaviour == other.behaviour && imageFolder == other.imageFolder && targetCount == other.targetCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Variable) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (key != null ? key.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (value != null ? value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) behaviour;
                hashCode = (hashCode * 397) ^ (imageFolder != null ? imageFolder.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ targetCount;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Variable {key} ({value}, {behaviour})";
        }
    }
}