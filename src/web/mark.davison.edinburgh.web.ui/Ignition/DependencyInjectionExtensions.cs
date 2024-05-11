namespace mark.davison.edinburgh.web.ui.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseEdinburghWeb(this IServiceCollection services, IAuthenticationConfig authConfig)
    {
        services
            .UseEdinburghComponents(authConfig)
            .UseFluxorState(typeof(Program), typeof(FeaturesRootType))
            .UseClientRepository(WebConstants.ApiClientName, WebConstants.LocalBffRoot)
            .UseClientCQRS(typeof(Program), typeof(FeaturesRootType));

        return services;
    }
}
