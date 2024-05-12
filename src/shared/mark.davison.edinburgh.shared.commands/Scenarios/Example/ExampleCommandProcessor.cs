namespace mark.davison.edinburgh.shared.commands.Scenarios.Example;

public sealed class ExampleCommandProcessor : ICommandProcessor<ExampleCommandRequest, ExampleCommandResponse>
{
    public async Task<ExampleCommandResponse> ProcessAsync(ExampleCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return new ExampleCommandResponse
        {
            ResponseValue = $"{request.Value} + {request.Value} + from the api"
        };
    }
}