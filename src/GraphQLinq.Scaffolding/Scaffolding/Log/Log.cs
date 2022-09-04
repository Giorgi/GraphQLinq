using Spectre.Console;

namespace GraphQLinq.Scaffolding;

internal class Log
{
    public static void Line()
    {
        AnsiConsole.WriteLine("");
    }

    public static void Err(object o)
    {
        AnsiConsole.WriteLine("\u001b[31m" + o + "\u001b[0m");
    }

    public static void Warn(object o)
    {
        AnsiConsole.WriteLine("\u001b[33m" + o + "\u001b[33m");
    }

    public static void Inf(object o)
    {
        AnsiConsole.MarkupLine("[bold]" + o + "[/]");
    }

    public static void Success(object o)
    {
        AnsiConsole.WriteLine("\u001b[32m" + o + "\u001b[0m");
    }
    public static void Title(object o)
    {
        AnsiConsole.WriteLine("\u001b[33;1m" + o + "\u001b[0m");
        Line();
    }
}
