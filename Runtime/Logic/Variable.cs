using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Variable : ISavable
    {
        public enum Behaviour
        {
            Unrestricted = 0,
            Change_Higher = 1
        }

        public enum Order
        {
            Any = 0,
            Static = 1,
            Random = 2
        }

        [Header("Configuration")] public string key;
        public object value = false;
        public Behaviour behaviour = Behaviour.Unrestricted;
        public string imageFolder;
        public Order order;
        [DefaultValue(3)] public int targetCount = 3;
        [DefaultValue(true)] public bool resetOnCheckpoint = true;

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

        public Variable()
        {
        }

        public Variable(string key) : this()
        {
            this.key = key;
        }

        public Variable(string key, bool value, Behaviour behaviour = Behaviour.Unrestricted) : this(key)
        {
            this.value = value;
            this.behaviour = behaviour;
        }

        public float GetNumeric(object value)
        {
            if (value is float f) return f;
            if (value is double d) return (float) d;
            if (value is int i) return i;
            if (value is bool b) return b ? 1f : 0;
            if (value is string s)
            {
                float.TryParse(s, out float val);
                return val;
            }

            return 0;
        }

        public float GetNumeric()
        {
            return GetNumeric(value);
        }

        public bool GetBool(object value)
        {
            if (value is bool b) return b;
            if (value is float f) return f == 1f;
            if (value is double d) return d == 1d;
            if (value is int i) return i == 1;
            if (value is string s) return s == "1";

            return false;
        }

        public bool GetBool()
        {
            return GetBool(value);
        }

        public void Merge(Variable copyFrom)
        {
            value = copyFrom.value;
            behaviour = copyFrom.behaviour;
            imageFolder = copyFrom.imageFolder;
            order = copyFrom.order;
            targetCount = copyFrom.targetCount;
            targetOrder = copyFrom.targetOrder;
            currentOrder = copyFrom.currentOrder;
            everChanged = copyFrom.everChanged;
            resetOnCheckpoint = copyFrom.resetOnCheckpoint;

            // ensure we are always using floats when using decimals for compatibility (e.g. FlowCanvas sync)
            if (value is double) value = Convert.ChangeType(value, typeof(float));
        }

        public string GetPersistedState()
        {
            return SDKUtil.SerializeObject(this);
        }

        public void LoadPersistedState(string state)
        {
            Variable v = SDKUtil.DeserializeObject<Variable>(state);
            Merge(v);
        }

        protected bool Equals(Variable other)
        {
            return key == other.key && value == other.value && behaviour == other.behaviour
                   && imageFolder == other.imageFolder && targetCount == other.targetCount
                   && order == other.order && resetOnCheckpoint == other.resetOnCheckpoint;
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
                hashCode = (hashCode * 397) ^ order.GetHashCode();
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