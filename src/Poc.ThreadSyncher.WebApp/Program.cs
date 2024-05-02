using ThreadSyncher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddThreadSyncher();

var app = builder.Build();

app.MapGet("/pull/{key}", async (string key, IThreadSyncher<string> syncher) =>
{
    var ctx = syncher.InitContext(key, force: true);

    var result = await ctx.PullAndCloseAsync(TimeSpan.FromSeconds(10));

    return Results.Ok(new { key, result, Environment.CurrentManagedThreadId });
});

app.MapPost("/push/{key}", async (string key, IThreadSyncher<string> syncher) =>
{
    var ctx = syncher.GetContext(key);

    await ctx.PushAsync($"Hello from thread {Environment.CurrentManagedThreadId} !");

    return Results.Ok();
});

app.Run();