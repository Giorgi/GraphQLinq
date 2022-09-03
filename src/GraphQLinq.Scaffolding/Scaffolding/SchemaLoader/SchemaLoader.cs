using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Spectre.Console;

namespace GraphQLinq.Scaffolding;

internal static class SchemaLoader
{
    internal static async Task<RootSchemaObject> Load(Uri uri, string basicAuthUsername, string basicAuthPassword, bool saveJsonSchema, string contextNamePath, List<string> headerKeys, List<string> headerValues)
    {
        try
        {
            return await AnsiConsole.Status().StartAsync("Performing introspection", async ctx =>
            {
                AnsiConsole.WriteLine("Running introspection query ...");

                string schemaJson = await GetJsonSchema(uri, basicAuthUsername, basicAuthPassword, headerKeys, headerValues);

                if (uri.IsFile && !uri.OriginalString.ToLower().StartsWith("http"))
                {
                    if (saveJsonSchema)
                    {
                        AnsiConsole.WriteLine("Argument: saveJsonSchema is true, but you read a local json file, you already have it");
                    }
                }
                else if (saveJsonSchema)
                {
                    System.Threading.Thread.Sleep(10);
                    File.WriteAllText(contextNamePath + ".json", schemaJson);
                    AnsiConsole.WriteLine("SaveJsonSchema: completed...");
                }

                return JsonSerializer.Deserialize<RootSchemaObject>(schemaJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            });
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    static async Task<string> GetJsonSchema(Uri uri, string usr, string pwd, List<string> headerKeys, List<string> headerValues)
    {
        if (uri.IsFile && !uri.OriginalString.ToLower().StartsWith("http"))
        {
            AnsiConsole.WriteLine("Reading and deserializing schema information from local file " + uri.OriginalString + "...");

            return await File.ReadAllTextAsync(uri.OriginalString);
        }

        if(!uri.OriginalString.EndsWith("/graphql"))
        {
            AnsiConsole.WriteLine("Warning: your uri " + uri.OriginalString + " does not end with /graphql, most graphql servers uses the '/graphql' endpoint as a listener for incoming queries");
        }

        using var httpClient = new HttpClient();
        if (usr != null && usr.Length > 0 && pwd != null && pwd.Length > 0)
        {
            AnsiConsole.WriteLine("Schema will be queried with basic authorization, with user: " + usr);

            var basicToken = GetBasicAuthorization(usr, pwd);
            httpClient.DefaultRequestHeaders.Add("Authentication", basicToken);
        }

        if(headerKeys != null && headerKeys.Count > 0)
        {
            AnsiConsole.WriteLine("Schema will be queried with custom headers, amount of headers: " + headerKeys.Count);
           
            for (int i = 0; i < headerKeys.Count; i++)
            {
                var val = headerValues != null && i < headerValues.Count ? headerValues[i] : "";

                httpClient.DefaultRequestHeaders.Add(headerKeys[i], val);
            }
        }

        AnsiConsole.WriteLine("Reading and deserializing schema information from url " + uri.OriginalString + "...");

        using var responseMessage = await httpClient.PostAsJsonAsync(uri, new { query = Globals.IntrospectionQuery });

        if(!responseMessage.IsSuccessStatusCode)
        {
            var errorMsg = "Error reading schema from " + uri + "\nStatusCode: " + responseMessage.StatusCode + ", " + responseMessage.ReasonPhrase;

            errorMsg += "\n" + await responseMessage.Content.ReadAsStringAsync();

            AnsiConsole.WriteLine(errorMsg);
        }
        var responseString = await responseMessage.Content.ReadAsStringAsync();

        if (responseString.Contains("<title>"))
            throw new Exception("Error reading schema from " + uri + " it returned html: " + responseString);

        return responseString;
    }

    static string GetBasicAuthorization(string usr, string pwd)
    {
        return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(usr + ":" + pwd));
    }
}
