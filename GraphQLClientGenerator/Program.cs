using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
                             enumValues{
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
            var httpClient = new HttpClient();
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/json");

                //var downloadString = webClient.UploadString("https://api.spacex.land/graphql", query);
            var responseMessage = await httpClient.PostAsJsonAsync("https://api.spacex.land/graphql", new { query = IntrospectionQuery });
            var downloadString = await responseMessage.Content.ReadAsStringAsync();
            var rootObject = JsonSerializer.Deserialize<RootSchemaObject>(downloadString);

            var codeGenerationOptions = new CodeGenerationOptions
            {
                Namespace = "HSL",
                OutputDirectory = @"E:\src\GraphQLinq\GraphQLinq.Demo\HSL"
            };
            var graphQLClassesGenerator = new GraphQLClassesGenerator(codeGenerationOptions);
            graphQLClassesGenerator.GenerateClasses(rootObject.Data.Schema);
        }
    }

    class CodeGenerationOptions
    {
        public string OutputDirectory { get; set; }
        public string Namespace { get; set; }
    }
}
