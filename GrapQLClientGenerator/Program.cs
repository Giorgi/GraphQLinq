using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GrapQLClientGenerator
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
