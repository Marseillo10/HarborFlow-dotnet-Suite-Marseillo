using HarborFlowSuite.Abstractions.Services;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using HarborFlowSuite.Server.Hubs;
using HarborFlowSuite.Server.Services;
using HarborFlowSuite.Application.Services; // Added for service implementations
using FirebaseAdmin; // Added for FirebaseApp
using Google.Apis.Auth.OAuth2; // Added for GoogleCredential
using Microsoft.IdentityModel.Tokens; // Added for TokenValidationParameters

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()

    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddHostedService<AisDataService>();

// Configure PostgreSQL and ApplicationDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<ApplicationDbContext>(connectionString);

// Register services
builder.Services.AddScoped<IAuthService, HarborFlowSuite.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IDashboardService, HarborFlowSuite.Infrastructure.Services.DashboardService>();
builder.Services.AddScoped<IServiceRequestService, HarborFlowSuite.Infrastructure.Services.ServiceRequestService>();
builder.Services.AddScoped<IUserProfileService, HarborFlowSuite.Application.Services.UserProfileService>();

// Configure GFW API client
builder.Services.AddHttpClient<IGfwMetadataService, GfwMetadataService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["GfwApiBaseUrl"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["GfwApiKey"]}");
});

// Configure Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase-adminsdk.json"),
});

// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/harborflow-aef5d";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/harborflow-aef5d",
            ValidateAudience = true,
            ValidAudience = "harborflow-aef5d",
            ValidateLifetime = true
        };
    });

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient",
        policy =>
        {
            policy.WithOrigins("https://localhost:7163", "http://localhost:5205") // Adjust ports if needed
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Apply migrations on startup in development environment
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Attempting to connect to the database and apply migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Database connection successful and migrations applied.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}

// app.UseHttpsRedirection();

app.UseCors("AllowClient");

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AisHub>("/aisHub");
app.MapFallbackToFile("index.html");

app.Run();

