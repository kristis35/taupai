using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SaskaituAgentas.Api.Configuration;

/// <summary>Prideda JWT „Bearer“ schemą į OpenAPI, kad Swagger UI rodytų „Authorize“ mygtuką.</summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private const string SchemeId = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeId] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Prieigos žetonas iš POST /api/auth/login arba /api/auth/register.",
        };
        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeId, document)] = [],
        });
        return Task.CompletedTask;
    }
}
