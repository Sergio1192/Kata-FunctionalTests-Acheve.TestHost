using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FunctionalTests.Infraestructure;

public class ApiFixture : WebApplicationFactory<StartupTest>
{
    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        return WebHost.CreateDefaultBuilder();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseStartup<StartupTest>()
            .UseSolutionRelativeContentRoot("src")
            .UseTestServer();
    }
}