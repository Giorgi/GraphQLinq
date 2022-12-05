using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GraphQLinq.Scaffolding
{
    public class RootSchemaObject
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("__schema")]
        public Schema Schema { get; set; }
    }

    public class Schema
    {
        public List<GraphqlType> Types { get; set; }
        public GraphqlType QueryType { get; set; }
        public GraphqlType MutationType { get; set; }
        public GraphqlType? SubscriptionType { get; set; }
    }

    public class GraphqlType : BaseInfo
    {
        public TypeKind Kind { get; set; }
        public List<EnumValue> EnumValues { get; set; }
        public List<Field> Fields { get; set; }
        public List<Field> InputFields { get; set; }
        public List<GraphqlType> Interfaces { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum TypeKind
    {
        [EnumMember(Value = "LIST")]
        List,
        [EnumMember(Value = "NON_NULL")]
        NonNull,
        [EnumMember(Value = "SCALAR")]
        Scalar,
        [EnumMember(Value = "OBJECT")]
        Object,
        [EnumMember(Value = "INTERFACE")]
        Interface,
        [EnumMember(Value = "UNION")]
        Union,
        [EnumMember(Value = "ENUM")]
        Enum,
        [EnumMember(Value = "INPUT_OBJECT")]
        InputObject
    }

    public class EnumValue
    {
        public string Name { get; set; }
    }

    public class Field : BaseInfo
    {
        public FieldType Type { get; set; }
        public List<Arg> Args { get; set; }
    }

    public class FieldType
    {
        public TypeKind Kind { get; set; }
        public string? Name { get; set; }
        public FieldType? OfType { get; set; }
    }

    public class Arg : BaseInfo
    {
        public FieldType Type { get; set; }
    }

    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public class BaseInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}