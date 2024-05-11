namespace mark.davison.edinburgh.shared.queries.Scenarios.Startup;

public sealed class StartupQueryProcessor : IQueryProcessor<StartupQueryRequest, StartupQueryResponse>
{
    private readonly EdinburghDbContext _dbContext;

    public StartupQueryProcessor(EdinburghDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartupQueryResponse> ProcessAsync(StartupQueryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var projects = await _dbContext
            .Set<Project>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new StartupQueryResponse
        {
            Value = new()
            {
                ProjectNames = [.. projects.Select(_ => _.Name), "Default Project"]
            }
        };
    }
}
