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
            .AddLogging()
            .AddJwtAuth2(AppSettings.AUTH)
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddHealthCheckServices<InitializationHostedService>() // TODO: Rename Configure to Add
            .AddCors()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc))
            .AddDatabase<EdinburghDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(PostgresContextFactory), typeof(SqliteContextFactory)); // TODO: UseDatabase should return IServiceCollection, and rename Use to Add

        services.MapCQRSServer();// TODO: UseCQRSServer should return IServiceCollection, and rename Use to Add

        if (!string.IsNullOrEmpty(AppSettings.REDIS.HOST)) // TODO: Add a helperfor this
        {

            var config = new ConfigurationOptions
            {
                EndPoints = { AppSettings.REDIS.HOST + ":" + AppSettings.REDIS.PORT },
                Password = AppSettings.REDIS.PASSWORD
            };

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config);
            services
                .AddStackExchangeRedisCache(_ =>
                {
                    _.InstanceName = AppSettings.SECTION + "_" + (AppSettings.PRODUCTION_MODE ? "PROD_" : "DEV_");
                    _.Configuration = redis.Configuration;
                })
                .AddSingleton(redis);
            //.AddSingleton<IRedisService, RedisService>();
        }

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
            .UseMiddleware<PopulateUserContextMiddleware2>()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapHealthChecks();

                endpoints
                    .ConfigureCQRSEndpoints(); // TODO: Rename Configure to Map

                endpoints
                    .MapGet<User>() // TODO: Rename Use to Map
                    .MapGetById<User>()// TODO: Rename Use to Map
                    .MapPost<User>();// TODO: Rename Use to Map
            });
    }
}
