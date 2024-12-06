using SprintSimulationBackend.Hubs;
using SprintSimulationBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register the list of runners
builder.Services.AddSingleton(new List<Runner>
{
    new Runner { Name = "Runner 1", Speed = 0, Position = 0, Acceleration = 0.2, ReactionTime = 0.1 },
    new Runner { Name = "Runner 2", Speed = 0, Position = 0, Acceleration = 0.25, ReactionTime = 0.2 }
});

builder.Services.AddSingleton<RaceSimulationService>();
builder.Services.AddSignalR();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // React frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // Allow credentials (cookies, headers, etc.)
    });
});



var app = builder.Build();

// Ensure CORS policy is applied before routing
app.UseCors("AllowFrontend");

app.UseRouting();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<RaceHub>("/racehub");
});

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
