namespace mark.davison.edinburgh.web.features.Store.StartupUseCase;

[FeatureState]
public sealed class StartupState
{
    public ReadOnlyCollection<string> ProjectNames { get; }

    public StartupState() : this([])
    {

    }

    public StartupState(IEnumerable<string> projectNames)
    {
        ProjectNames = new([.. projectNames]);
    }
}
