using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JOS.Tokens.HttpClients
{
    public class CompanyHttpClient5
    {
        private static readonly SemaphoreSlim AccessTokenSemaphore;
        private static AccessToken? _accessToken;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CompanyHttpClient5> _logger;

        static CompanyHttpClient5()
        {
            _accessToken = null!;
            AccessTokenSemaphore = new SemaphoreSlim(1, 1);
        }

        public CompanyHttpClient5(HttpClient httpClient, IConfiguration configuration, ILogger<CompanyHttpClient5> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            _logger.LogInformation("Fetching company...");
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
            try
            {
                await AccessTokenSemaphore.WaitAsync();

                if (_accessToken is { Expired: false })
                {
                    return _accessToken;
                }
         
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
            finally
            {
                AccessTokenSemaphore.Release(1);
            }
        }
    }
}
