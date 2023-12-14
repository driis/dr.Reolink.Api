using System.Text.Json.Serialization;

namespace dr.Reolink.Api.Camera.Model;

static class Command
{
    public static ReoCommand<T> Create<T>(string cmd, T param) => new ReoCommand<T>(cmd, param);
}


// Reolink Camera serializing types - generic 
record ReoCommand<T>(string Cmd, T Param);
record ReoResponse<T>(string Cmd, int Code, T Value);
record ReoConfirm(int RspCode);

// User / Authentication
record ReoUserWrapper(ReoUser User)
{
    [JsonPropertyName("User")] public ReoUser User { get; init; } = User;
};
record ReoUser(int Version, string UserName, string Password);

record ReoTokenWrapper(ReoTokenValue Token)
{
    [JsonPropertyName("Token")] public ReoTokenValue Token { get; init; } = Token;
};
record ReoTokenValue(int LeaseTime, string Name);

// White LED - Floodlight 
record ReoWhiteLedWrapper(ReoWhiteLedParam WhiteLed)
{
    [JsonPropertyName("WhiteLed")] public ReoWhiteLedParam WhiteLed { get; init; } = WhiteLed;
};
record ReoWhiteLedParam(int Channel, int State);
