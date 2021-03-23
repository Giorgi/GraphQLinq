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
                new Argument<string>("output", "Output folder"),
                new Option<string>(new []{ "--namespace", "-n" }, "Namespace of generated classes"),
            };

            generate.Handler = CommandHandler.Create<string, string, string, IConsole>(HandleGenerate);

            await generate.InvokeAsync(args);
        }

        private static async Task HandleGenerate(string endpoint, string output, string? @namespace, IConsole console)
        {
            var httpClient = new HttpClient();
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/json");

            //var downloadString = webClient.UploadString("https://api.spacex.land/graphql", query);
            var responseMessage = await httpClient.PostAsJsonAsync(endpoint, new { query = IntrospectionQuery });
            var schemaJson = await responseMessage.Content.ReadAsStringAsync();
            var rootObject = JsonSerializer.Deserialize<RootSchemaObject>(schemaJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var codeGenerationOptions = new CodeGenerationOptions
            {
                Namespace = @namespace ?? "",
                NormalizeCasing = true,
                OutputDirectory = output
            };
            var graphQLClassesGenerator = new GraphQLClassesGenerator(codeGenerationOptions);
            graphQLClassesGenerator.GenerateClasses(rootObject.Data.Schema);
        }
    }

    class CodeGenerationOptions
    {
        public string OutputDirectory { get; set; }
        public string Namespace { get; set; }
        public bool NormalizeCasing { get; set; }
    }
}
