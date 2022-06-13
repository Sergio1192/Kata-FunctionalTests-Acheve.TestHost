using Host.Infraestructure.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class AddOpenApiExtensions
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddOpenApiSecurity();
            });

            return services;
        }

        private static void AddOpenApiSecurity(this SwaggerGenOptions options)
        {
            const string TYPE_AUTHENTICATION_TOKEN = "Bearer";

            OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Description = "Por favor introduce en el campo la palabra 'Bearer' seguida por un espacio y el token.",
                Name = "Authorization",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = TYPE_AUTHENTICATION_TOKEN
                },
                Scheme = TYPE_AUTHENTICATION_TOKEN
            };

            options.AddSecurityDefinition(TYPE_AUTHENTICATION_TOKEN, openApiSecurityScheme);

            options.OperationFilter<AuthorizeOperationFilter>(openApiSecurityScheme);
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    internal static class UseOpenApiExtensions
    {
        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
        {

            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}