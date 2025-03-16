using ImageUploader.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddScoped<IImageUploaderService, ImageUploaderService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // Serve files from wwwroot
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();