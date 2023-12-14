using dr.Reolink.Api.Camera;
using Microsoft.Extensions.Options;

namespace dr.Reolink.Tests.Api.Camera;

public class CameraClientTests
{
    private CameraClient Sut { get; set; } = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://192.168.171.3")
        };
        Sut = new CameraClient(client,
            new OptionsWrapper<CameraApiOptions>(new CameraApiOptions() {UserName = "admin", Password = Environment.GetEnvironmentVariable("REOLINK__PASSWORD")!}));
    }
    
    [Test][Ignore("Currently running against my real camera, might want to mock these out, eventually")]
    public async Task Authenticate_CanAuthenticateWithValidConfig()
    {
        var token = await Sut.Authenticate(CancellationToken.None);
        
        Assert.That(token.Expires, Is.GreaterThan(DateTimeOffset.Now.AddSeconds(120)));
        Assert.That(token.Value, Is.Not.Empty);
    }

    [Test][Ignore("Currently running against my real camera, might want to mock these out, eventually")]
    public async Task FloodlightOn_TurnsFloodlightOn_AndOff()
    {
        var ct = CancellationToken.None;
        await Sut.SetFloodLight(true, ct);
        await Task.Delay(5000);     // To begin with running against real cam - don't want to flicker too fast 
        await Sut.SetFloodLight(false, ct);
    }
}