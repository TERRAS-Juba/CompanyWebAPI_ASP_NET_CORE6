using System.Net;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Formatters;
using CompanyEmployees.ServicesConfigurations;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Npgsql;
using Repository;
using Repository.DataShaping;

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

// Added support for Marvin cache library
builder.Services.ConfigureHttpCacheHeaders();

// Add services to the container.
builder.Services.ConfigureControllersBehaviour();

// Add custom Output Formaters
builder.Services.ConfigureCustomeOutputFormaters();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();