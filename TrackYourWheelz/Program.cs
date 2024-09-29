using Microsoft.EntityFrameworkCore;
using TrackYourWheelz.EntityFrameworkCore.Context;
using TrackYourWheelz.Repositorys;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TrackYourWheelzDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrackYourWheelzDbContext"));
});


builder.Services.AddTransient<ITcpServerRepository, TcpServerRepository>();

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
