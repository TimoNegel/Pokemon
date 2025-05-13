using System.Text.Json;

namespace Backend
{
    public static class StringHelper
    {
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static string? GetJsonString(this JsonElement element, params string[] path)
        {
            foreach (var prop in path)
            {
                if (
                    element.ValueKind == JsonValueKind.Object
                    && element.TryGetProperty(prop, out var next)
                )
                {
                    element = next;
                }
                else
                {
                    return null;
                }
            }

            return element.ValueKind == JsonValueKind.String ? element.GetString() : null;
        }
    }
}
