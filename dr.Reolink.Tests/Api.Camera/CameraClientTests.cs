using dr.Reolink.Api.Camera;
using Microsoft.Extensions.Options;

namespace dr.Reolink.Tests.Api.Camera;

public class CameraClientTests
{
    [Test]
    public async Task Authenticate_CanAuthenticateWithValidConfig()
    {
        string password = Environment.GetEnvironmentVariable("REOLINK__PASSWORD")!;
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://192.168.171.3")
        };

        var sut = new CameraClient(client,
            new OptionsWrapper<CameraApiOptions>(new CameraApiOptions() {UserName = "admin", Password = password}));

        var token = await sut.Authenticate(CancellationToken.None);
        
        Assert.That(token.Expires, Is.GreaterThan(DateTimeOffset.Now.AddSeconds(120)));
        Assert.That(token.Value, Is.Not.Empty);
    }
}