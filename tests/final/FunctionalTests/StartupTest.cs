using Acheve.AspNetCore.TestHost.Security;
using Acheve.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FunctionalTests;

public class StartupTest
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(TestServerDefaults.AuthenticationScheme)
            .AddTestServer();

        services.AddApiServices();

        services.AddControllers()
            .AddApplicationPart(Assembly.Load(new AssemblyName(nameof(Api))));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
