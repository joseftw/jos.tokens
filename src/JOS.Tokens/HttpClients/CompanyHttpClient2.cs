using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace JOS.Tokens.HttpClients
{
    public class CompanyHttpClient2
    {
        private const string AccessToken = "super-secret-access-token-that-lives-forever";
        private readonly HttpClient _httpClient;

        public CompanyHttpClient2(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Company> Get(string companyName)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/companies/{companyName}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await using var responseContentStream = await response.Content.ReadAsStreamAsync();

            var company = await JsonSerializer.DeserializeAsync<Company>(
                responseContentStream,
                DefaultJsonSerializerOptions.Options);

            return company ?? throw new Exception("Failed to deserialize company");
        }
    }
}
