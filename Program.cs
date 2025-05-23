<<<<<<< HEAD
using WerewolfGame.Services; // Thêm using để dùng GameService

var builder = WebApplication.CreateBuilder(args);

// Đăng ký dịch vụ GameService và controller
builder.Services.AddControllers();                    // Cho phép sử dụng [ApiController]
builder.Services.AddSingleton<GameService>();         // Đăng ký GameService dùng DI

// Thêm CORS cho phép tất cả
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger/OpenAPI
=======
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Đảm bảo đã thêm Controllers

// Nếu bạn dùng Swagger cho API
>>>>>>> 425881246f1c57b1a76b797c3b74ea22b367db38
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

<<<<<<< HEAD
// Cấu hình pipeline
=======
// Configure the HTTP request pipeline.
>>>>>>> 425881246f1c57b1a76b797c3b74ea22b367db38
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

<<<<<<< HEAD
app.UseCors("AllowAll");


// Map controller endpoints
app.MapControllers();  // Quan trọng để định tuyến các controller như GameController

// Demo route mặc định
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
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

app.Run();

// WeatherForecast record vẫn giữ nguyên
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
=======
// Các API controller sẽ được nhận diện từ đây
app.MapControllers(); // Đây là bước quan trọng để ánh xạ các controller API

app.Run();
>>>>>>> 425881246f1c57b1a76b797c3b74ea22b367db38
