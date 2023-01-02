namespace GraphQLinq.Scaffolding.Extensions
{
    internal static class StringExtensions
    {
        internal static string ToCamelCase(this string input)
        {
            if (char.IsLower(input[0]))
            {
                return input;
            }
            return input.Substring(0, 1).ToLower() + input.Substring(1);
        }

        internal static string ToPascalCase(this string input)
        {
            if (char.IsUpper(input[0]))
            {
                return input;
            }
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        internal static string NormalizeIfNeeded(this string input, GeneratorOptions options)
        {
            if (options.SingleOutput) return input;

            return options.NormalizeCasing ? input.ToPascalCase() : input;
        }
    }
}