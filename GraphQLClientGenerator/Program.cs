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
                                     }
                                   }
                                 }
                               }
                               args {
                                 name
                                 description
                                 type {
                                   name
                                 }
                               }
                             }
                           }
                           queryType {
                             name
                           }
                        }
                      }";

            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/graphql");
            var downloadString = webClient.UploadString("https://api.digitransit.fi/routing/v1/routers/finland/index/graphql", query);
            var rootObject = JsonConvert.DeserializeObject<RootSchemaObject>(downloadString);

            var codeGenerationOptions = new CodeGenerationOptions
            {
                Namespace = "HSL",
                OutputDirectory = @"E:\src\GraphQLinq\GraphQLinq.Demo\HSL"
            };
            var graphQLClassesGenerator = new GraphQLClassesGenerator();
            graphQLClassesGenerator.GenerateClasses(rootObject.Data.Schema, codeGenerationOptions);
        }
    }

    class CodeGenerationOptions
    {
        public string OutputDirectory { get; set; }
        public string Namespace { get; set; }
    }
}
