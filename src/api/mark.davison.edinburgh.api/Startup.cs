using mark.davison.edinburgh.api.migrations.postgres;
using mark.davison.edinburgh.api.migrations.sqlite;

namespace mark.davison.edinburgh.api;

[UseCQRSServer(typeof(DtosRootType), typeof(CommandsRootType), typeof(QueriesRootType))]
public class Startup
{
    public IConfiguration Configuration { get; }

    public AppSettings AppSettings { get; set; } = null!;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.ConfigureSettingsServices<AppSettings>(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        Console.WriteLine(AppSettings.DumpAppSettings(AppSettings.PRODUCTION_MODE));

        services
            .AddCors()
            .AddLogging()
            .AddJwtAuth(AppSettings.AUTH)
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddHealthCheckServices<InitializationHostedService>()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc))
            .AddDatabase<EdinburghDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(PostgresContextFactory), typeof(SqliteContextFactory))
            .AddCoreDbContext<EdinburghDbContext>()
            .AddCQRSServer()
            .AddRedis(AppSettings.REDIS, AppSettings.SECTION, AppSettings.PRODUCTION_MODE);

    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
            builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(_ => true) // TODO: Config driven
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader());

        app.UseHttpsRedirection();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseMiddleware<PopulateUserContextMiddleware>()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapHealthChecks();

                endpoints
                    .MapGet<User>()
                    .MapGetById<User>()
                    .MapPost<User>();

                endpoints
                    .MapCQRSEndpoints();

            });
    }
}
