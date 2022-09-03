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
            AnsiConsole.MarkupLine("[bold]Welcome to GraphQL Client Scaffolding tool[/]");
            AnsiConsole.WriteLine();

            var outputFolder = Path.IsPathRooted(outputDirectory) ? outputDirectory : Path.Combine(Environment.CurrentDirectory, outputDirectory);
            AnsiConsole.MarkupLine("Scaffolding GraphQL client code for [bold]{0}[/] to [bold]{1}[/]", uri, outputFolder);

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            string contextNamePath = GetContextNamePath(contextName, outputFolder);

            if (saveGraphQlQuery)
            {
                File.WriteAllText(contextNamePath + ".query", Globals.IntrospectionQuery);
                AnsiConsole.MarkupLine("Wrote the .query file to: " + contextNamePath + ".query");
                AnsiConsole.MarkupLine("[bold]Scaffolding complete[/]");
                AnsiConsole.MarkupLine("[bold]Use the .query file within graphql playground to get the json text that this tool can convert to C# files[/]");
                return;
            }
            RootSchemaObject schema = await SchemaLoader.Load(uri, basicAuthUsername, basicAuthPassword, saveJsonSchema, contextNamePath, headerKeys, headerValues);

            AnsiConsole.WriteLine();




            var contextClassFullName = AnsiConsole.Status().Start($"Scaffolding GraphQL client code {uri}", statusContext =>
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

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Scaffolding complete[/]");
            AnsiConsole.MarkupLine("Use [bold]{0}[/] to run strongly typed LINQ queries", contextClassFullName);
        }
        catch (Exception ex)
        {
            AnsiConsole.Write(ex.ToString());

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
