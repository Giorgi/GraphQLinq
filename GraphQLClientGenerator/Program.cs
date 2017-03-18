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
                             }
                           }
                        }
                      }";

            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/graphql");
            var downloadString = webClient.UploadString("https://api.digitransit.fi/routing/v1/routers/finland/index/graphql", query);
            var rootObject = JsonConvert.DeserializeObject<RootObject>(downloadString);

            var graphQLClassesGenerator = new GraphQLClassesGenerator();
            graphQLClassesGenerator.GenerateClasses(rootObject);
        }
    }
}
