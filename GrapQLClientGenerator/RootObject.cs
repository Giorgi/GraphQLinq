using System.Collections.Generic;
using Newtonsoft.Json;

namespace GrapQLClientGenerator
{
    public class RootObject
    {
        public Data Data { get; set; }
    }
    
    public class Data
    {
        [JsonProperty("__schema")]
        public Schema Schema { get; set; }
    }

    public class Schema
    {
        public List<Type> Types { get; set; }
    }

    public class Type
    {
        public string Name { get; set; }
        public string Kind { get; set; }
        public string Description { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public string Description { get; set; }
    }

    public class FieldType
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public OfType OfType { get; set; }
    }

    public class OfType
    {
        public string Kind { get; set; }
        public string Name { get; set; }
    }
}