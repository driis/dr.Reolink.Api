using dr.Reolink.Api.Camera;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configure the environment
builder.Services.Configure<CameraApiOptions>(builder.Configuration.GetSection("reolink"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<CameraClient>()
    .AddHttpClient<CameraClient>((s, cli) =>
    {
        cli.BaseAddress = new Uri(
            s.GetRequiredService<IOptions<CameraApiOptions>>().Value.Validate().Endpoint
        );
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Self test configuration before starting API
var config = app.Services.GetRequiredService<IOptions<CameraApiOptions>>().Value;
config.Validate();

// API endpoints
app.MapGet("/api/floodlight/on", (CameraClient cli, CancellationToken ct) => cli.SetFloodLight(true, ct))
    .WithName("Floodlight On")
    .WithDescription("Turns the floodlight on")
    .WithOpenApi();

app.MapGet("/api/floodlight/off", (CameraClient cli, CancellationToken ct) => cli.SetFloodLight(false, ct))
    .WithName("Floodlight Off")
    .WithDescription("Turns the floodlight off")
    .WithOpenApi();

// Start the API
app.Run();
