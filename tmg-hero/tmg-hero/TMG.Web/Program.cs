using tmg_hero.Engine;
using TMG.Web.Hubs;
using TMG.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<GameController>();
builder.Services.AddSingleton<GameService>();
builder.Services.AddSignalR();

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseStaticFiles();

// API endpoints
app.MapGet("/api/status", async (GameService gameService) =>
{
    var status = await gameService.GetStatusAsync();
    return Results.Ok(status);
});

app.MapPost("/api/initialize", async (GameService gameService, string? saveData) =>
{
    try
    {
        await gameService.InitializeAsync(saveData);
        return Results.Ok(new { success = true, message = "Game initialized successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

app.MapPost("/api/start", async (GameService gameService) =>
{
    try
    {
        await gameService.StartAsync();
        return Results.Ok(new { success = true, message = "Bot started" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

app.MapPost("/api/stop", async (GameService gameService) =>
{
    try
    {
        await gameService.StopAsync();
        return Results.Ok(new { success = true, message = "Bot stopped" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

app.MapGet("/api/save", async (GameService gameService) =>
{
    try
    {
        var saveData = await gameService.GetSaveAsync();
        return Results.Ok(new { success = true, saveData });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = ex.Message });
    }
});

// SignalR hub
app.MapHub<GameHub>("/gamehub");

// Default route
app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine("TMG Hero Web Interface starting...");
Console.WriteLine("Open http://localhost:5000 in your browser");

app.Run();