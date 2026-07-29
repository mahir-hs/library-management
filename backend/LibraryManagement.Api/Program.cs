using LibraryManagement.Infrastructure.DependencyInjection;
using LibraryManagement.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;

// ============ SERILOG CONFIGURATION ============
// Configure structured logging BEFORE building the host
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "LibraryManagementApi")
    .CreateLogger();

try
{
    Log.Information("Starting Library Management API...");

    // ============ BUILD CONFIGURATION ============
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog to the host builder
    builder.Host.UseSerilog();

    // ============ REGISTER SERVICES (BEFORE Build()) ============
    // ORDER MATTERS: Add services in dependency order

    // 1. Add Controllers
    builder.Services.AddControllers();

    // 2. Add Health Checks (with DbContext verification)
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>(name: "database");

    // 3. Add Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Library Management API",
            Version = "v1",
            Description = "RESTful API for Library Management System",
            Contact = new OpenApiContact
            {
                Name = "Library Management Team",
                Email = "support@librarymgmt.com"
            }
        });

        // 1. Define the Security Scheme
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        });

        // 2. Add Security Requirement using OpenApiSecuritySchemeReference
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        // Include XML comments...
        var xmlFiles = new[] { "LibraryManagement.Domain.xml", "LibraryManagement.Application.xml", "LibraryManagement.Api.xml" };
        foreach (var xmlFile in xmlFiles)
        {
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        }
    });

    // 4. Add Infrastructure Services (DbContext, Repositories, UnitOfWork)
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // 5. Add CORS (before UseRouting)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200",      // Angular dev server
                    "http://localhost:3000",      // Alternative port
                    "http://127.0.0.1:4200")      // 127.0.0.1 variant
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // 6. Add Authentication (JWT Bearer)
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secret = jwtSettings["Secret"]
        ?? throw new InvalidOperationException("JWT Secret is not configured in appsettings.json");
    var issuer = jwtSettings["Issuer"] ?? "LibraryManagementApi";
    var audience = jwtSettings["Audience"] ?? "LibraryManagementClient";

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.Zero  // No tolerance for clock skew
            };

            // Log JWT validation errors
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Warning("JWT authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Log.Information("JWT token validated for user: {UserId}",
                        context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown");
                    return Task.CompletedTask;
                }
            };
        });

    // 7. Add Authorization policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("AdminOrLibrarian", policy =>
            policy.RequireRole("Admin", "Librarian"));

        options.AddPolicy("MemberOnly", policy =>
            policy.RequireRole("Member"));

        options.AddPolicy("AnyAuthenticated", policy =>
            policy.RequireRole("Admin", "Librarian", "Member"));
    });

    // ============ BUILD THE APPLICATION ============
    var app = builder.Build();

    // ============ MIDDLEWARE PIPELINE ============

    // 1. Use Serilog request logging (should be early)
    app.UseSerilogRequestLogging();

    // 2. Swagger/OpenAPI (development only)
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1");
            options.RoutePrefix = string.Empty;  // Serve Swagger at root (http://localhost:5243/)
            options.DisplayOperationId();
            options.DefaultModelsExpandDepth(2);
            options.DefaultModelExpandDepth(2);
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            options.EnableFilter();
            options.ShowExtensions();
        });

        Log.Information("Swagger UI available at http://localhost:5243/");
    }

    // 3. HTTPS Redirect (production)
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    // 4. CORS (must be before routing)
    app.UseCors("AllowFrontend");

    // 5. Routing
    app.UseRouting();

    // 6. Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // 7. Health Check Middleware Endpoint
    app.MapHealthChecks("/health");

    // 8. Map Controllers
    app.MapControllers();

    // 9. Root fallback endpoint
    app.MapGet("/info", () => Results.Ok(new { message = "Library Management API v1 - Use /swagger for documentation" }))
        .WithName("GetInfo");

    // ============ STARTUP LOGGING ============
    Log.Information("Application configuration complete");
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);

    // ============ DATABASE INITIALIZATION (Development Only) ============
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            Log.Information("Database context initialized. Run 'dotnet ef database update' to apply migrations.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database initialization failed. Ensure database connection string is correct.");
        }
    }

    // ============ GRACEFUL SHUTDOWN ============
    app.Lifetime.ApplicationStopping.Register(() =>
    {
        Log.Information("Library Management API is shutting down...");
        Log.CloseAndFlush();
    });

    // ============ RUN APPLICATION ============
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}