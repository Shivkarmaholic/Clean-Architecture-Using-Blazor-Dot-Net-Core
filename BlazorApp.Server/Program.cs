using BlazorApp.Application;
using BlazorApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register Application Layer services
builder.Services.AddApplication();

// Register Infrastructure Layer services
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorWasmPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7180", "http://localhost:5180")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // During development, you can use a more permissive CORS policy
    app.UseCors(policy => 
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
}
else
{
    // In production, use the specific policy
    app.UseCors("BlazorWasmPolicy");
}

app.UseHttpsRedirection();

// Add controller endpoints
app.MapControllers();


await app.Services.InitializeDatabaseAsync();


app.Run();
