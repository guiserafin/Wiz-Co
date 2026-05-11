namespace WizCo.Api.Configurations;

using Microsoft.OpenApi.Models;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WizCo — API de Pedidos",
                Version = "v1",
                Description = "API REST para gerenciamento de pedidos e itens de pedido."
            });
        });

        return services;
    }
}
