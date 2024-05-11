namespace mark.davison.edinburgh.web.features.Store.StartupUseCase;

public static class StartupReducers
{
    [ReducerMethod]
    public static StartupState FetchStartupActionResponse(StartupState state, FetchStartupActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new StartupState(response.Value.ProjectNames);
        }

        return state;
    }
}
