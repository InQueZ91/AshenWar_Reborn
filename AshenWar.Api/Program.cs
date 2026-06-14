using System.Text;
using AshenWar.Api.Auth;
using AshenWar.Api.Hubs;
using AshenWar.Application;
using AshenWar.Application.Contracts.Notifications;
using AshenWar.Infrastructure;
using AshenWar.Infrastructure.Definitions;
using AshenWar.Infrastructure.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);
builder.Services.AddSingleton<IGameNotifier, GameNotifier>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// JWT Authentication
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings is missing from configuration. Add it to user-secrets.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Load game definitions from disk
var definitionLoader = app.Services.GetRequiredService<DefinitionLoader>();
definitionLoader.LoadAll(Path.Combine(app.Environment.ContentRootPath, "Definitions"));

// --- Middleware pipeline configuration---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/hubs/game");

app.Run();