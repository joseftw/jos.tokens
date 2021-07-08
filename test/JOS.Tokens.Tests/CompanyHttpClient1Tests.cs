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
    public class CompanyHttpClient1Tests
    {
        private readonly CompanyHttpClient1 _sut;

        public CompanyHttpClient1Tests()
        {
            var getCompanyResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"name\": \"JEHO Consulting\"}", Encoding.UTF8, "application/json")
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(getCompanyResponse))
            {
                BaseAddress = new Uri("https://localhost.local")
            };
            _sut = new CompanyHttpClient1(httpClient);
        }

        [Fact]
        public async Task ShouldReturnCompanyGivenValidResponse()
        {
            var result = await _sut.Get("JEHO Consulting");

            result.Name.ShouldBe("JEHO Consulting");
        }
    }
}
