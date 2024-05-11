namespace mark.davison.edinburgh.shared.queries.Scenarios.Startup;

public sealed class StartupQueryHandler : ValidateAndProcessQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    public StartupQueryHandler(
        IQueryProcessor<StartupQueryRequest, StartupQueryResponse> processor
    ) : base(
        processor)
    {
    }
}