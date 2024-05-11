namespace mark.davison.edinburgh.web.components.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseEdinburghComponents(
        this IServiceCollection services,
        IAuthenticationConfig authConfig)
    {
        services.UseCommonClient(authConfig, typeof(Routes));
        //services.AddSingleton<IClientJobHttpRepository, ClientJobHttpRepository>();
        return services;
    }
}
