using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace dr.Reolink.Api.Camera;

public class CameraClient
{
    private readonly HttpClient _http;
    private readonly CameraApiOptionsValidated _options;

    public CameraClient(HttpClient http, IOptions<CameraApiOptions> options)
    {
        _http = http;
        _options = options.Value.Validate();
    }

    public async Task<AuthenticationToken> Authenticate(CancellationToken ct)
    {
        var cmd = Command.Create("Login", new ReoUserWrapper(new (0, _options.UserName, _options.Password)));
        var response = await Send(cmd, ct);
        var responseStr = await response.Content.ReadAsStringAsync(ct);
        var responseObj = JsonSerializer.Deserialize<ReoResponse<ReoTokenWrapper>[]>(responseStr, new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        var token = responseObj?.FirstOrDefault()?.Value.Token;
        if (token == null || token.LeaseTime <= 0 || String.IsNullOrWhiteSpace(token.Name))
            throw new ApplicationException($"Invalid token from Camera API: {token}");
        return new AuthenticationToken(DateTimeOffset.Now.AddSeconds(token.LeaseTime), token.Name);
    }

    private async Task<HttpResponseMessage> Send<T>(ReoCommand<T> cmd, CancellationToken ct, bool authenticated = true, bool expectSuccess = true)
    {
        var payload = new[] {cmd};
        var response = await _http.PostAsJsonAsync($"cgi-bin/api.cgi?cmd={cmd.Cmd}", payload, cancellationToken: ct);
        if (expectSuccess)
            response.EnsureSuccessStatusCode();
        return response;
    }
}

public class CameraApiOptions
{
    public string? UserName { get; set; }
    public string? Password { get; set; }

    internal CameraApiOptionsValidated Validate()
    {
        if (UserName != null && Password != null)
        {
            return new CameraApiOptionsValidated(UserName!, Password!);
        }

        throw new ApplicationException($"Invalid configuration of CameraApiOptions. {new {UserName, Password}}");
    }
}

record CameraApiOptionsValidated(string UserName, string Password);

public record AuthenticationToken(DateTimeOffset Expires, string Value)
{ }



static class Command
{
    public static ReoCommand<T> Create<T>(string cmd, T param) => new ReoCommand<T>(cmd, param);
}

// Reolink Camera serializing types
record ReoCommand<T>(string Cmd, T Param);

record ReoUserWrapper(ReoUser User)
{
    [JsonPropertyName("User")] public ReoUser User { get; init; } = User;
};

record ReoUser(int Version, string UserName, string Password);
record ReoResponse<T>(string Cmd, int Code, T Value);

record ReoTokenWrapper(ReoTokenValue Token)
{
    [JsonPropertyName("Token")] public ReoTokenValue Token { get; init; } = Token;
};
record ReoTokenValue(int LeaseTime, string Name);