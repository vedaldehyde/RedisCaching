using RedisCaching.BL;
using RedisCaching.Interfaces;
using RedisCaching.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Interfaces
builder.Services.AddScoped<IMoviesBL, MoviesBL>();

builder.Services.AddSingleton<CacheConfigService>();

//Redis
builder.Services.AddDistributedRedisCache(
    options =>
    {
        options.Configuration = "localhost:6379";
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
