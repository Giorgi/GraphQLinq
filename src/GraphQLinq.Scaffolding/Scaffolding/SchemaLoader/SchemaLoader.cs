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
                Log.Inf("Running introspection query ...");

                string schemaJson = await GetJsonSchema(uri, basicAuthUsername, basicAuthPassword, headerKeys, headerValues);

                if (uri.IsFile && !uri.OriginalString.ToLower().StartsWith("http"))
                {
                    if (saveJsonSchema)
                    {
                        Log.Warn("Argument: saveJsonSchema is true, but you read a local json file, you already have it");
                    }
                }
                else if (saveJsonSchema)
                {
                    System.Threading.Thread.Sleep(10);
                    File.WriteAllText(contextNamePath + ".json", schemaJson);
                    Log.Success("Completed step: saving json schema");
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
            Log.Inf("Reading schema information from local file " + uri.OriginalString + "...");

            return await File.ReadAllTextAsync(uri.OriginalString);
        }

        if(!uri.OriginalString.EndsWith("/graphql"))
        {
            Log.Warn("Warning: your uri " + uri.OriginalString + " does not end with /graphql, most graphql servers uses the '/graphql' endpoint as a listener for incoming queries");
            Log.Warn("Warning: if you see an error 'NotFound', try add /graphql to the url");
        }

        using var httpClient = new HttpClient();
        if (usr != null && usr.Length > 0 && pwd != null && pwd.Length > 0)
        {
            Log.Inf("Schema will be queried with basic authorization, with user: " + usr);

            var basicToken = GetBasicAuthorization(usr, pwd);
            httpClient.DefaultRequestHeaders.Add("Authentication", basicToken);
        }

        if(headerKeys != null && headerKeys.Count > 0)
        {
            Log.Inf("Schema will be queried with " + headerKeys.Count + " custom headers");
           
            for (int i = 0; i < headerKeys.Count; i++)
            {
                var val = headerValues != null && i < headerValues.Count ? headerValues[i] : "";

                httpClient.DefaultRequestHeaders.Add(headerKeys[i], val);
            }
        }

        Log.Inf("Reading schema information from query at url " + uri.OriginalString + "...");

        using var responseMessage = await httpClient.PostAsJsonAsync(uri, new { query = Globals.IntrospectionQuery });

        var responseString = await responseMessage.Content.ReadAsStringAsync();

        if (!responseMessage.IsSuccessStatusCode)
        {
            var errorMsg = "Error reading schema from " + uri + "\nStatusCode: " + responseMessage.StatusCode + ", " + responseMessage.ReasonPhrase;
            Log.Err(errorMsg);
            Log.Line();

            Log.Warn("Http response is:");
            Log.Warn(responseString);
        }

        if (responseString.Contains("<title>"))
        {
            Log.Err("Response contains html, so endpoint responds, but it might be wrong path or port, or completely wrong endpoint (url/server) since it returned html");
            throw new Exception("Cannot continue");
        }

        return responseString;
    }

    static string GetBasicAuthorization(string usr, string pwd)
    {
        return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(usr + ":" + pwd));
    }
}
