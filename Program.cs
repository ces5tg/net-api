var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/api/{id}", async (HttpContext context, string id) =>
{
    // Obtener una instancia de HttpClient a través de la inyección de dependencias
    var httpClient = context.RequestServices.GetRequiredService<HttpClient>();

    // Construir la URL de la API de Tenor con el identificador proporcionado
    var tenorUrl = $"https://g.tenor.com/v1/gifs?ids={id}&key=LIVDSRZULELA&media_filter=gif";

    // Hacer una solicitud HTTP a la API de Tenor
    var response = await httpClient.GetAsync(tenorUrl);
    response.EnsureSuccessStatusCode();

    // Leer la respuesta como una cadena JSON
    var responseBody = await response.Content.ReadAsStringAsync();

    // Devolver la respuesta de la API de Tenor
    return responseBody;
})
.WithName("TenorSearch")
.WithOpenApi();

app.UseCors();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}