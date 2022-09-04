using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Spectre.Console;

namespace GraphQLinq.Scaffolding;

public class Program
{
    static async Task Main(string[] args)
    {
        var command = Arguments.GetCommandWithArguments();

        command.Handler = CommandHandler.Create<Arguments>(Run);

        await command.InvokeAsync(args);
    }

    static async Task Run(Arguments args)
    {
        var contextName = args.contextName;
        var uri = args.uri;
        var @namespace = args.@namespace;
        var basicAuthUsername = args.basicAuthUsername;
        var basicAuthPassword = args.basicAuthPassword;
        var outputDirectory = args.outputDirectory;
        var outputFileName = args.outputFileName;
        var saveJsonSchema = args.saveJsonSchema;
        var saveGraphQlQuery = args.saveGraphQlQuery;
        var headerKeys = GetSemiColonSeparatedList(args.headerKeys);
        var headerValues = GetSemiColonSeparatedList(args.headerValues);

        var console = args.console;

        if (contextName == null || contextName == "")
            contextName = "Query";

        if (contextName.EndsWith("Context"))
            contextName = contextName.Replace("Context", "");

        if (contextName == "")
            contextName = "Query";

        if (@namespace == null || @namespace == "")
            @namespace = "Graphql.Types";

        try
        {
            Log.Title("Welcome to GraphQL Client Scaffolding tool");

            var outputFolder = Path.IsPathRooted(outputDirectory) ? outputDirectory : Path.Combine(Environment.CurrentDirectory, outputDirectory);
            Log.Inf(string.Format("Scaffolding schema {0} to folder: {1}", uri, outputFolder));

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            string contextNamePath = GetContextNamePath(contextName, outputFolder);

            if (saveGraphQlQuery)
            {
                File.WriteAllText(contextNamePath + ".query", Globals.IntrospectionQuery);
                Log.Inf("Wrote .query file to: " + contextNamePath + ".query");
                Log.Line();
                Log.Success("Completed - use .query file inside graphql playground to get the json schema");
                return;
            }

            RootSchemaObject schema = await SchemaLoader.Load(uri, basicAuthUsername, basicAuthPassword, saveJsonSchema, contextNamePath, headerKeys, headerValues);

            var contextClassFullName = AnsiConsole.Status().Start($"Scaffolding {uri}", statusContext =>
            {
                var options = new GeneratorOptions
                {
                    Namespace = @namespace,
                    NormalizeCasing = true,
                    OutputDirectory = outputFolder,
                    OutputFileName = outputFileName,
                    SingleOutput = outputFileName != null && outputFileName.Length > 0,
                    ContextName = contextName
                };

                var generator = new Generator(options);

                return generator.GenerateClient(schema.Data.Schema, uri.AbsoluteUri);
            });

            Log.Line();

            Log.Success("Complete");
            Log.Inf("Use " + contextClassFullName + " to run strongly typed LINQ queries in your C# code");
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    static List<string> GetSemiColonSeparatedList(string text)
    {
        if(text == null || text == " " || text.Length < 1)
        {
            return null;
        }

        text = text.Trim();

        if (text.EndsWith(";"))
            text = text.Substring(0, text.Length - 1);

        return text.Split(';').ToList();
    }

    static string GetContextNamePath(string contextName, string outputFolder)
    {
        var contextFullPath = outputFolder;

        if (contextFullPath.EndsWith("\\"))
            contextFullPath += contextName;
        else
            contextFullPath += "\\" + contextName;

        if (contextFullPath.EndsWith(".cs"))
            contextFullPath = contextFullPath.Replace(".cs", "");

        return contextFullPath;
    }
}
