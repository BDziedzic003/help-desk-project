using Microsoft.OpenApi;

namespace Helpdesk.Web.Infrastructure;

public static class SwaggerExtensions
{
    private const string DocumentName = "v1";
    private const string XmlDocumentationFile = "Helpdesk.Web.xml";

    public static IServiceCollection AddHelpdeskSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo
            {
                Title = "Helpdesk API",
                Version = DocumentName,
                Description = "API systemu zgłoszeń. Uwierzytelnianie odbywa się ciasteczkiem: " +
                              "wywołaj najpierw POST /api/auth/login, a przeglądarka dołączy ciasteczko do kolejnych zapytań."
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, XmlDocumentationFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    public static WebApplication UseHelpdeskSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{DocumentName}/swagger.json", "Helpdesk API v1");
            options.DocumentTitle = "Helpdesk API";
        });

        return app;
    }
}
