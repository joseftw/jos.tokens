using System;
using System.Threading.Tasks;
using JOS.Tokens.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JOS.Tokens
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();
            var environment = GetEnvironment(configuration);

            var builder = new HostBuilder()
                .UseEnvironment(environment)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(configuration);
                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<FakeHttpMessageHandler>();
                    services.AddHttpClient<CompanyHttpClient1>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost.local");
                    }).ConfigurePrimaryHttpMessageHandler<FakeHttpMessageHandler>();
                    services.AddHttpClient<CompanyHttpClient2>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost.local");
                    }).ConfigurePrimaryHttpMessageHandler<FakeHttpMessageHandler>();
                    services.AddHttpClient<CompanyHttpClient3>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost.local");
                    }).ConfigurePrimaryHttpMessageHandler<FakeHttpMessageHandler>();
                    services.AddHttpClient<CompanyHttpClient4>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost.local");
                    }).ConfigurePrimaryHttpMessageHandler<FakeHttpMessageHandler>();
                    services.AddHttpClient<CompanyHttpClient5>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost.local");
                    }).ConfigurePrimaryHttpMessageHandler<FakeHttpMessageHandler>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    _ = new SerilogConfigurator(hostingContext.Configuration, hostingContext.HostingEnvironment)
                        .Configure();
                    logging.AddSerilog(dispose: true);
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateOnBuild = true;
                    options.ValidateScopes = true;
                });

            var host = builder.Build();
            await host.RunAsync();
        }

        private static string GetEnvironment(IConfiguration configuration)
        {
            var environment = configuration.GetValue<string>("environment");

            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = Environments.Development;
            }

            return environment;
        }
    }
}
