using System.Text.Json;

namespace JOS.Tokens
{
    public static class DefaultJsonSerializerOptions
    {
        static DefaultJsonSerializerOptions()
        {
            Options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public static JsonSerializerOptions Options { get; }
    }
}
