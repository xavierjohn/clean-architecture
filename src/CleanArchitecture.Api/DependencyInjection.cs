namespace CleanArchitecture.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.ToString());
        });
        services.AddProblemDetails();

        return services;
    }
}