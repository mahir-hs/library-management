using LibraryManagement.Api.Extensions;
using LibraryManagement.API.Middleware;
using LibraryManagement.Infrastructure.DependencyInjection;
using Serilog;

// ============ LOGGING ============
Log.Logger = SerilogExtensions.ConfigureSerilog().CreateLogger();
Log.Information("Starting Library Management API...");

// ============ HOST BUILD ============
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ============ SERVICE REGISTRATION ============
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddHealthCheckConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCorsConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

// ============ BUILD & PIPELINE ============
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Management API v1");
        options.RoutePrefix = string.Empty;
        options.DisplayOperationId();
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.EnableFilter();
        options.ShowExtensions();
    });

    Log.Information("Swagger UI available at http://localhost:5243/");
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/info", () => Results.Ok(new { message = "Library Management API v1 - Use /swagger for documentation" }))
    .WithName("GetInfo");

Log.Information("Application configuration complete");
Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);

// ============ DATABASE INITIALIZATION ============
await app.InitializeDatabaseAsync();

// ============ RUN ============
await app.RunAsync();
