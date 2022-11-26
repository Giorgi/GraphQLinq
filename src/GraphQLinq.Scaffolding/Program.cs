using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Spectre.Console;

namespace GraphQLinq.Scaffolding
{
    class Program
    {
        private const string IntrospectionQuery = @"{
  __schema {
    types {
      name
      interfaces {
        name
      }
      description
      kind
      enumValues {
        name
      }
      description
      fields {
        name
        description
        type {
          name
          kind
          ofType {
            name
            kind
            ofType {
              name
              kind
              ofType {
                name
                kind
                ofType {
                  name
                  kind
                  ofType {
                    name
                    kind
                  }
                }
              }
            }
          }
        }
        args {
          name
          description
          type {
            kind
            name
            description
            ofType {
              name
              kind
              ofType {
                name
                kind
                ofType {
                  name
                  kind
                }
              }
            }
          }
        }
      }
      inputFields {
        name
        description
        type {
          name
          kind
          ofType {
            name
            kind
            ofType {
              name
              kind
              ofType {
                name
                kind
              }
            }
          }
        }
      }
    }
    queryType {
      name
    }
    mutationType {
      name
    }
    subscriptionType {
      name
    }
  }
}
";

        private static async Task Main(string[] args)
        {
            var generate = new RootCommand("Scaffold GraphQL client code")
            {
                new Argument<Uri>("endpoint", "Endpoint of the GraphQL service"),
                new Option<string>(new []{ "--output", "-o" }, () => "", "Output folder"),
                new Option<string>(new []{ "--namespace", "-n" }, () => "","Namespace of generated classes"),
                new Option<string>(new []{ "--context", "-c" }, () => "Query","Name of the generated context classes"),
            };

            generate.Handler = CommandHandler.Create<Uri, string, string, string, IConsole>(HandleGenerate);

            await generate.InvokeAsync(args);
        }

        private static async Task HandleGenerate(Uri endpoint, string output, string @namespace, string context, IConsole console)
        {
            //var webClient = new WebClient();
            //webClient.Headers.Add("Content-Type", "application/json");
            //var downloadString = webClient.UploadString("endpoint", query);
            AnsiConsole.MarkupLine("[bold]Welcome to GraphQL Client Scaffolding tool[/]");
            AnsiConsole.WriteLine();

            string outputFolder = Path.IsPathRooted(output) ? output : Path.Combine(Environment.CurrentDirectory, output);

            AnsiConsole.MarkupLine("Scaffolding GraphQL client code for [bold]{0}[/] to [bold]{1}[/]", endpoint, outputFolder);

            var data = await AnsiConsole.Status().StartAsync("Performing introspection", async ctx =>
            {
                try
                {

                    AnsiConsole.WriteLine("Running introspection query ...");
                    GraphQLHttpClient client = new GraphQLHttpClient(endpoint, new SystemTextJsonSerializer());
                    var request = new GraphQLRequest
                    {
                        Query = IntrospectionQuery
                    };
                    var responseMessage = await client.SendQueryAsync<Data>(request);

                    AnsiConsole.WriteLine("Reading and deserializing schema information ...");
                    return responseMessage.Data;
                    //var schemaJson = await responseMessage;
                    //return JsonSerializer.Deserialize<RootSchemaObject>(schemaJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            AnsiConsole.WriteLine();

            var contextClassFullName = AnsiConsole.Status().Start($"Scaffolding GraphQL client code {endpoint}", statusContext =>
            {
                var codeGenerationOptions = new CodeGenerationOptions
                {
                    Namespace = @namespace,
                    NormalizeCasing = true,
                    OutputDirectory = outputFolder,
                    ContextName = context
                };

                var graphQLClassesGenerator = new GraphQLClassesGenerator(codeGenerationOptions);
                return graphQLClassesGenerator.GenerateClient(data.Schema, endpoint.AbsoluteUri);
            });

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Scaffolding complete[/]");
            AnsiConsole.MarkupLine("Use [bold]{0}[/] to run strongly typed LINQ queries", contextClassFullName);
        }
    }

    class CodeGenerationOptions
    {
        public string? Namespace { get; set; } = "";
        public string ContextName { get; set; } = "";
        public string OutputDirectory { get; set; } = "";
        public bool NormalizeCasing { get; set; }
    }
}
