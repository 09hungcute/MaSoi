var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Đảm bảo đã thêm Controllers

// Nếu bạn dùng Swagger cho API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Các API controller sẽ được nhận diện từ đây
app.MapControllers(); // Đây là bước quan trọng để ánh xạ các controller API

app.Run();
