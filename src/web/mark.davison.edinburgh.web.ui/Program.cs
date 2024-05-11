var authConfig = new AuthenticationConfig();
authConfig.SetBffBase(WebConstants.LocalBffRoot);

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    })
    .UseEdinburghWeb(authConfig);

await builder.Build().RunAsync();
