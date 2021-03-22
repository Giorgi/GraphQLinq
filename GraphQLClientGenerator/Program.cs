using System.Net;
using Newtonsoft.Json;

namespace GraphQLClientGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var query = @"{
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

            var httpClient = new HttpClient();
            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/json");

            try
            {
                //var downloadString = webClient.UploadString("https://api.spacex.land/graphql", query);
                var responseMessage = PostAsJsonAsync(httpClient, "https://api.spacex.land/graphql", new { query = query }).GetAwaiter().GetResult();
                var downloadString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var rootObject = JsonConvert.DeserializeObject<RootSchemaObject>(downloadString);

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
