using System.Text.RegularExpressions;

namespace Shared
{
    public static class StringUtil
    {
        // Converts "camelCaseText" to "camel_case_text"
        public static string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Match lowercase letters followed by an uppercase letter
            // and insert an underscore between them.
            return Regex.Replace(text, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        // Converts "snake_case_text" to "snakeCaseText"
        public static string ToCamelCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return Regex.Replace(text, @"_([a-z0-9])", match =>
                match.Groups[1].Value.ToUpper()
            );
        }
    }
}

