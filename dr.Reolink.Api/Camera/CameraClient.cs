using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using dr.Reolink.Api.Camera.Model;
using Microsoft.Extensions.Options;

namespace dr.Reolink.Api.Camera;

public class CameraClient
{
    private readonly HttpClient _http;
    private readonly CameraApiOptionsValidated _options;
    private readonly AuthenticationHelper _authentication;

    public CameraClient(HttpClient http, IOptions<CameraApiOptions> options)
    {
        _http = http;
        _options = options.Value.Validate();
        _authentication = new AuthenticationHelper(Authenticate);
    }

    public async Task<AuthenticationToken> Authenticate(CancellationToken ct)
    {
        var cmd = Command.Create("Login", new ReoUserWrapper(new (0, _options.UserName, _options.Password)));
        var response = await Send(cmd, ct, authenticated:false).AsReoResponse<ReoTokenWrapper>();
        var token = response.Token;
        if (token == null || token.LeaseTime <= 0 || String.IsNullOrWhiteSpace(token.Name))
            throw new ApplicationException($"Invalid token from Camera API: {token}");
        return new AuthenticationToken(DateTimeOffset.Now.AddSeconds(token.LeaseTime), token.Name);
    }

    public async Task SetFloodLight(bool on, CancellationToken ct)
    {
        var cmd = Command.Create("SetWhiteLed", new ReoWhiteLedWrapper(new ReoWhiteLedParam(0, on ? 1 : 0)));
        var response  = await Send(cmd, ct)
            .AsReoResponse<ReoConfirm>();
        if (response.RspCode != 200)
        {
            throw new ApplicationException($"Unexpected RspCode from API {response}");
        }
    }

    private async Task<HttpResponseMessage> Send<T>(ReoCommand<T> cmd, CancellationToken ct, bool authenticated = true, bool expectSuccess = true)
    {
        var payload = new[] {cmd};
        var token = authenticated ? $"&token={(await _authentication.GetToken(ct)).Value}" : "";
        var response = await _http.PostAsJsonAsync($"cgi-bin/api.cgi?cmd={cmd.Cmd}{token}", payload, cancellationToken: ct);
        if (expectSuccess)
            response.EnsureSuccessStatusCode();
        return response;
    }
}