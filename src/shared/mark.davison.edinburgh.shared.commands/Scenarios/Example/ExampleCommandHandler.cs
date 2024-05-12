namespace mark.davison.edinburgh.shared.commands.Scenarios.Example;

public sealed class ExampleCommandHandler : ValidateAndProcessCommandHandler<ExampleCommandRequest, ExampleCommandResponse>
{
    public ExampleCommandHandler(ICommandProcessor<ExampleCommandRequest, ExampleCommandResponse> processor) : base(processor)
    {
    }
}
