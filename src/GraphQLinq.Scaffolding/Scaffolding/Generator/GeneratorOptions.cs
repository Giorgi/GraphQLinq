using System.Collections.Generic;

namespace GraphQLinq.Scaffolding;

class GeneratorOptions
{
    public string? Namespace { get; set; } = "";
    public string ContextName { get; set; } = "";
    public string OutputDirectory { get; set; } = "";
    public string OutputFileName { get; set; } = "";
    public bool NormalizeCasing { get; set; }

    public bool SingleOutput { get; set; }
}

