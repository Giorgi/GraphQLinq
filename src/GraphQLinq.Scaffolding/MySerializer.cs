﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Scaffolding;

public class MySerializer : IGraphQLWebsocketJsonSerializer
{
    public static JsonSerializerSettings DefaultJsonSerializerSettings => new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver { IgnoreIsSpecifiedMembers = true },
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Converters = { new ConstantCaseEnumConverter() }
    };

    public JsonSerializerSettings JsonSerializerSettings { get; }

    public MySerializer() : this(DefaultJsonSerializerSettings) { }

    public MySerializer(Action<JsonSerializerSettings> configure) : this(configure.AndReturn(DefaultJsonSerializerSettings)) { }

    public MySerializer(JsonSerializerSettings jsonSerializerSettings)
    {
        JsonSerializerSettings = jsonSerializerSettings;
        ConfigureMandatorySerializerOptions();
    }

    // deserialize extensions to Dictionary<string, object>
    private void ConfigureMandatorySerializerOptions() => JsonSerializerSettings.Converters.Insert(0, new MapConverter());

    public string SerializeToString(GraphQLRequest request) => JsonConvert.SerializeObject(request, JsonSerializerSettings);

    public byte[] SerializeToBytes(GraphQLWebSocketRequest request)
    {
        var json = JsonConvert.SerializeObject(request, JsonSerializerSettings);
        return Encoding.UTF8.GetBytes(json);
    }

    public Task<WebsocketMessageWrapper> DeserializeToWebsocketResponseWrapperAsync(Stream stream) => DeserializeFromUtf8Stream<WebsocketMessageWrapper>(stream);

    public GraphQLWebSocketResponse<GraphQLResponse<TResponse>> DeserializeToWebsocketResponse<TResponse>(byte[] bytes) =>
        JsonConvert.DeserializeObject<GraphQLWebSocketResponse<GraphQLResponse<TResponse>>>(Encoding.UTF8.GetString(bytes),
            JsonSerializerSettings);

    public Task<GraphQLResponse<TResponse>> DeserializeFromUtf8StreamAsync<TResponse>(Stream stream, CancellationToken cancellationToken) => DeserializeFromUtf8Stream<GraphQLResponse<TResponse>>(stream);

    private Task<T> DeserializeFromUtf8Stream<T>(Stream stream)
    {
        using var sr = new StreamReader(stream);
        using JsonReader reader = new JsonTextReader(sr);
        var serializer = JsonSerializer.Create(JsonSerializerSettings);
        return Task.FromResult(serializer.Deserialize<T>(reader));
    }
}