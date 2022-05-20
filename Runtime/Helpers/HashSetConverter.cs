using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace traVRsal.SDK
{
    [Serializable]
    public class HashSetConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HashSet<string>);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            // not needed, serialized like a list
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray ja = JArray.Load(reader);
            return new HashSet<string>(ja.Values<string>());
        }

        public override bool CanWrite => false;
    }
}