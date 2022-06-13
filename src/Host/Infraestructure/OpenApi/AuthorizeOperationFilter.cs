using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Host.Infraestructure.OpenApi;

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    private readonly string _securitySchemeName;

    public AuthorizeOperationFilter(string securitySchemeName)
    {
        _securitySchemeName = securitySchemeName;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.ApiDescription.CustomAttributes().OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>()
            {
                new OpenApiSecurityRequirement()
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = _securitySchemeName
                        }
                    }] = new List<string>()
                }
            };
        }
    }
}
