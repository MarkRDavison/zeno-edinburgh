namespace mark.davison.edinburgh.shared.models.dto.Scenarios.Commands.Example;

[PostRequest(Path = "example-command")]
public sealed class ExampleCommandRequest : ICommand<ExampleCommandRequest, ExampleCommandResponse>
{
    public string Value { get; set; } = string.Empty;
}
