using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphQLClientGenerator
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
            var generate = new RootCommand
            {
                new Argument<string>("endpoint", "Endpoint of the GraphQL service"),
                new Option<string>(new []{ "--output", "-o" }, "Output folder"),
                new Option<string>(new []{ "--namespace", "-n" }, "Namespace of generated classes"),
                new Option<string>(new []{ "--context", "-c" }, "Name of the generated context classes"),
            };

            generate.Handler = CommandHandler.Create<string, string, string, string, IConsole>(HandleGenerate);

            await generate.InvokeAsync(args);
        }

        private static async Task HandleGenerate(string endpoint, string? output, string? @namespace, string? context, IConsole console)
        {
            //var webClient = new WebClient();
            //webClient.Headers.Add("Content-Type", "application/json");
            //var downloadString = webClient.UploadString("endpoint", query);
            Console.WriteLine("Scaffolding GraphQL client for {0}", endpoint);

            using var httpClient = new HttpClient();
            Console.WriteLine("Running introspection query");

            using var responseMessage = await httpClient.PostAsJsonAsync(endpoint, new {query = IntrospectionQuery});
            string schemaJson = await responseMessage.Content.ReadAsStringAsync();

            var rootObject = JsonSerializer.Deserialize<RootSchemaObject>(schemaJson, new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

            Console.WriteLine("Scaffolding client code");

            var codeGenerationOptions = new CodeGenerationOptions
            {
                Namespace = @namespace ?? "",
                NormalizeCasing = true,
                OutputDirectory = string.IsNullOrEmpty(output) ? Environment.CurrentDirectory : output,
                ContextName = context ?? "Query"
            };

            var graphQLClassesGenerator = new GraphQLClassesGenerator(codeGenerationOptions);
            graphQLClassesGenerator.GenerateClient(rootObject.Data.Schema);

            Console.WriteLine("Scaffolding complete");
            Console.ReadKey();
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
