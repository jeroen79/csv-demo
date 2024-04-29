using RestJsonProvider.Generators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    // We want to keep our fields capital case in this exxample
    options.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

var generator = new FakeDataGenerator();

app.MapGet("/api/accounts", () =>
{
    var data = generator.GenrateBankAccounts();

    return data;
});

app.Run();