using System.Net;
using AspNetCoreRateLimit;
using CompanyEmployees.ServicesConfigurations;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using NLog.Web;
using AuthenticationManager = Repository.AuthenticationManager;

//====================================================================
//                              Builder
//====================================================================
// Logger
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

// Web application builder
var builder = WebApplication.CreateBuilder(args);

// Posgtres Database connection
builder.ConfigurePostgresDatabaseConnection("PostgreSqlConnection","CompanyEmployees");

// Added support for caching
builder.Services.ConfigureResponseCaching();

// Added memory cache for rate limiting and throttling
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
// Add configuration for rateLimitation
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

// Added support for Marvin cache library
builder.Services.ConfigureHttpCacheHeaders();

// Add services to the container.
builder.Services.ConfigureControllersBehaviour();

// Add custom Output Formaters
builder.Services.ConfigureCustomOutputFormaters();

// NLog: Setup NLog for Dependency injection
builder.ConfigureLogging();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors
builder.Services.ConfigureCorsPolicy();

// Added support for custome filters
builder.Services.ConfigureFilters();

// Added support for API versioning
builder.Services.ConfigureAPIVersionning();

// Added support for DataShaper
builder.Services.ConfigureDIForDataShaper();

// DI AutoMapper
builder.Services.ConfigureDIForAutoMapper();

// DI IRepositoryManager => RepositoryManager
builder.Services.ConfigureDIForRepositoryBase();

// Add support for asp .net core Identity
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();

// Add support to JWT Authentification
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddScoped<IAuthenticationManager,AuthenticationManager>();
//====================================================================
//                              App
//====================================================================
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

// Catching errors
app.UseExceptionHandler(appError => appError.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            logger.Error($"Something went wrong: {contextFeature.Error}");
            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error.",
                Description = contextFeature.Error.ToString()
            }.ToString());
        }
    })
);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseResponseCaching();

app.UseHttpCacheHeaders();

app.UseIpRateLimiting();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();