using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace JOS.Tokens.HttpClients
{
    public class CompanyHttpClient4
    {
        private static AccessToken? _accessToken;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;

        static CompanyHttpClient4()
        {
            _accessToken = null!;
        }

        public CompanyHttpClient4(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _clientId = config.GetValue<string>("CompanyApi:ClientId")!;
            ArgumentException.ThrowIfNullOrEmpty(_clientId);
            _clientSecret = config.GetValue<string>("CompanyApi:ClientSecret")!;
            ArgumentException.ThrowIfNullOrEmpty(_clientSecret);
        }

        public async Task<AccessToken> GetAccessToken()
        {
            if (_accessToken is { Expired: false })
            {
                return _accessToken;
            }

            _accessToken = await FetchToken();
            return _accessToken;
        }

        public async Task<Company> Get(string companyName)
        {
            var accessToken = await GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/companies/{companyName}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await using var responseContentStream = await response.Content.ReadAsStreamAsync();

            var company = await JsonSerializer.DeserializeAsync<Company>(
                responseContentStream,
                DefaultJsonSerializerOptions.Options);

            return company ?? throw new Exception("Failed to deserialize company");
        }

        private async Task<AccessToken> FetchToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/connect/token/")
            {
                Content = new FormUrlEncodedContent(new KeyValuePair<string?, string?>[]
                {
                    new("client_id", _clientId),
                    new("client_secret", _clientSecret),
                    new("scope", "company-api"),
                    new("grant_type", "client_credentials")
                })
            };

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            await using var responseContentStream = await response.Content.ReadAsStreamAsync();

            var accessToken = await JsonSerializer.DeserializeAsync<AccessToken>(
                responseContentStream,
                DefaultJsonSerializerOptions.Options);

            return accessToken ?? throw new Exception("Failed to deserialize access token");
        }
    }
}
