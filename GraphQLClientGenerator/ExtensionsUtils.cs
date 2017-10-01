namespace GraphQLClientGenerator
{
    static class ExtensionsUtils
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
    }
}