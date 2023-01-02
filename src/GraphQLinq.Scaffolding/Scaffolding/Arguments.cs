using System;
using System.CommandLine;

namespace GraphQLinq.Scaffolding;

public class Arguments
{
    /// <summary>
    /// Url to your /graphql endpoint which takes our schema query and returns the graphql schema as a whole
    /// 
    /// Optionally: uri is a local json file containing the schema already in a json format, skipping the 'query graphql for json schema' part, avoiding the httpclient request if you dont have access to it or struggle with bypassing its auth...
    /// </summary>
    /// <example>
    /// Url:
    /// <code>
    /// https://api.spacex.land/graphql
    /// </code>
    /// Full file path:
    /// <code>
    /// C:\\source\\api\\graphql-schema.json
    /// //A file equal to the 'Introspection query' result, if you run the 'Introspection query' in playground, the resulting json is 'graphql-schema.json'
    /// </code>
    /// </example>
    public Uri uri { get; set; }

    /// <summary>
    /// Folder directory: C:\source\project\grapql\
    /// 
    /// Required
    /// </summary>
    public string outputDirectory { get; set; }
    /// <summary>
    /// Optional: If set, then the whole schema will be converted to one C# file, else each schema type defined will have its own C# file
    /// </summary>
    public string outputFileName { get; set; }

    /// <summary>
    /// The namespace to put generated classes in under
    /// </summary>
    public string @namespace { get; set; }

    /// <summary>
    /// The context class name which will get an added suffix 'Context'
    /// 
    /// - The name of the class that will inherit the class GraphContext
    /// - Defaults to 'Query' if omitted
    /// </summary>
    public string contextName { get; set; }

    /// <summary>
    /// Username for basic authentication against the 'uri'
    /// 
    /// Optional: If the endpoint requires basic auth
    /// </summary>
    public string basicAuthUsername { get; set; }

    /// <summary>
    /// Username for basic authentication against the 'uri'
    /// 
    /// Optional: If the endpoint requires basic auth
    /// </summary>
    public string basicAuthPassword { get; set; }

    /// <summary>
    /// Optional: Save the converted graphql schema as json, to the same output directory, with same 'contextName'
    /// 
    /// For debugging purposes or any other reason one can think of...
    /// 
    /// Note: If true, the program still continues trying to convert the json schema into C# files
    /// </summary>
    public bool saveJsonSchema { get; set; }

    /// <summary>
    /// Optional: Save the graphql query schema as .query file to the same output directory, with same name as 'contextName'
    /// 
    /// Usage: You have access to the graphql playground, but cannot get this tool to access the api, you can then copy the query into playground and then store the .json result yourself, then run this tool against the json file to generate C# files
    /// 
    /// Note: if true then the program stops after exporting the query
    /// </summary>
    public bool saveGraphQlQuery { get; set; }

    /// <summary>
    /// Optional: Semi-colon separated header keys
    /// - the order of the keys must then match the order of the header values specified
    /// 
    /// Note: if you do not specify headerValues the header value for all keys passed will be "" (empty string)
    /// </summary>
    public string headerKeys { get; set; }

    /// <summary>
    /// Semi-colon separated header values, ex: val1;val 2;Val-3
    /// </summary>
    public string headerValues { get; set; }
    
    public IConsole console { get; set; }

    public static RootCommand GetCommandWithArguments()
    {
        return new RootCommand("Scaffold GraphQL client code")
        {
            new Argument<Uri>("uri", "A url against /graphql endpoint to query for schema of all types, or a local json file that already contains the schema for all types on the json format"),

            new Option<string>(new []{ "--outputDirectory", "-o" }, () => "",@"Optional: Folder directory, example: C:\source\project\grapql\, or relative to where you run the cmd from: Folder1/Folder2/Folder3 if empty uses the folder of where this tool is executed from"),
            new Option<string>(new []{ "--outputFileName", "-f" }, () => "","Optional: If set, then the whole schema will be converted to one C# file, else each schema type defined will have its own C# file"),

            new Option<string>(new []{ "--namespace", "-n" }, () => "Graphql.Types","The namespace to put generated classes in under"),
            new Option<string>(new []{ "--contextName", "-c" }, () => "Query","Name of the generated context classes\n - The name of the class that will inherit the class GraphContext\n - Defaults to 'Query' if omitted"),

            new Option<string>(new []{ "--basicAuthUsername", "-u" }, () => "","Username for basic authentication against the 'uri'\n - Optional: If the endpoint requires basic auth"),
            new Option<string>(new []{ "--basicAuthPassword", "-p" }, () => "","Password for basic authentication against the 'uri'\n - Optional: If the endpoint requires basic auth"),

            new Option<string>(new []{ "--headerKeys", "-hk" }, () => "","Semi-colon separated header values, ex: key1;key-2;key/3\n - Order of keys must match the order of header values specified\n - Note: if you do not specify headerValues, default header value for all keys is empty string"),
            new Option<string>(new []{ "--headerValues", "-hv" }, () => "","Semi-colon separated header values, ex: val1;val 2;Val-3"),

            new Option<string>(new []{ "--saveJsonSchema", "-j" }, () => "","Optional: Save the converted graphql schema as json, to the same output directory, with same 'contextName'\n - Note: If true, the program still continues trying to convert the json schema into C# files"),
            new Option<string>(new []{ "--saveGraphQlQuery", "-q" }, () => "","Optional: Save the graphql query schema as .query file to the same output directory, with same name as 'contextName'\n - Usage: if you have access to graphql playground, but cannot get this tool to access the api, you can then copy the query into playground and then store the .json result yourself, then run this tool against the json file to generate C# files\n - Note: if true then the program stops after exporting the query")
        };
    }
}