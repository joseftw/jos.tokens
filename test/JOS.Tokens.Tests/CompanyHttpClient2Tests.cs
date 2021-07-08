using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JOS.Tokens.HttpClients;
using Shouldly;
using Xunit;

namespace JOS.Tokens.Tests
{
    public class CompanyHttpClient2Tests
    {
        private const string GetCompanyJson = "{\"name\": \"JEHO Consulting\"}";

        [Fact]
        public async Task ShouldThrowExceptionIf401IsReturnedFromApi()
        {
            var httpClient = new HttpClient(
                new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Unauthorized)))
            {
                BaseAddress = new Uri("https://localhost.local")
            };
            var sut = new CompanyHttpClient2(httpClient);
        
            var exception = await Should.ThrowAsync<HttpRequestException>(() => sut.Get("JEHO Consulting"));

            exception.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            exception.Message.ShouldBe("Response status code does not indicate success: 401 (Unauthorized).");
        }

        [Fact]
        public async Task ShouldSendAuthorizationHeader()
        {
            var httpClient = new HttpClient(
                new FakeHttpMessageHandler(request =>
                {
                    request.Headers.ShouldContain(x => x.Key.Equals("Authorization"));
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"name\": \"JEHO Consulting\"}", Encoding.UTF8, "application/json")
                    });
                }))
            {
                BaseAddress = new Uri("https://localhost.local")
            };
            var sut = new CompanyHttpClient2(httpClient);

            var result = await sut.Get("JEHO Consulting");

            result.Name.ShouldBe("JEHO Consulting");
        }
    }
}
