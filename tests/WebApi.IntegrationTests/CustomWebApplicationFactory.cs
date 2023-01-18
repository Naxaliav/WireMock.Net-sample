using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.IntegrationTests;

public sealed class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Todo: replace already existing service or use something like options pattern and replace that.
            services.AddHttpClient<PetStoreClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5175");
            });
        });

        builder.UseEnvironment("Development");
    }
}