using Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Persistence;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "http://vm12.htl-leonding.ac.at"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add Entity Framework with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add MQTT Service
builder.Services.AddSingleton<IMqttService, MqttService>();

// Add Medication Reminder Background Service
builder.Services.AddHostedService<MedicationReminderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Only use HTTPS redirection in production, not in Docker containers
if (!app.Environment.EnvironmentName.Equals("Docker"))
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

// Start MQTT Service
var mqttService = app.Services.GetRequiredService<IMqttService>();
await mqttService.StartAsync();

app.Run();
