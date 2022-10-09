using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class TMProperty
    {
        public string name;
        public string type = "auto";
        public string value;

        public TMProperty()
        {
        }

        public TMProperty(string name, string type, string value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }

        public TMProperty(TMProperty copyFrom) : this()
        {
            name = copyFrom.name;
            type = copyFrom.type;
            value = copyFrom.value;
        }

        public override bool Equals(object obj)
        {
            return obj is TMProperty property &&
                   name == property.name &&
                   type == property.type &&
                   value == property.value;
        }

        public override int GetHashCode()
        {
            var hashCode = -1187636891;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(value);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{type} property '{name}' ({value})";
        }

        private void DetectType()
        {
            if (type.ToLowerInvariant() == "auto")
            {
                string valueLC = value.ToLowerInvariant();
                if (valueLC == "true" || valueLC == "false")
                {
                    type = "bool";
                }
                else if (value.Contains(".") && float.TryParse(value, out _))
                {
                    type = "float";
                }
                else if (int.TryParse(value, out _))
                {
                    type = "int";
                }
                else
                {
                    type = "string";
                }
            }
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            DetectType();
        }
    }
}