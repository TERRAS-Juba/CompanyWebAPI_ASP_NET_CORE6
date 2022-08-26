using System.Net;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Formatters;
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
var connectionBuilder = new NpgsqlConnectionStringBuilder();
connectionBuilder.ConnectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
builder.Services.AddDbContext<RepositoryContext>(o =>
    o.UseNpgsql(connectionBuilder.ConnectionString, b => b.MigrationsAssembly("CompanyEmployees")));

// Add services to the container.
builder.Services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;
        }
    ).AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();
    //.AddXmlDataContractSerializerFormatters()
// Added new OutputFormater wich is csv
builder.Services.AddMvc(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

// DI AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// DI IRepositoryManager => RepositoryManager
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors
builder.Services.AddCors(options =>
    options.AddPolicy("CorsPolicy", build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
 
// Replace error 400 to 422 when validation rules of incoming dto is not valide
// 400 => Bad request
// 422 => Unprocessable Entity
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Added support for personalised filter
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateCompanyExistsAttribute>();

// Added support for DataShaper
builder.Services.AddScoped<IDataShaper<EmployeeDto>,DataShaper<EmployeeDto>>();
builder.Services.AddScoped<IDataShaper<CompanyDto>,DataShaper<CompanyDto>>();
builder.Services.AddScoped<IDataShaper<CompanyJoinEmployeeDto>,DataShaper<CompanyJoinEmployeeDto>>();

// Added support for API versioning
builder.Services.AddApiVersioning(opt =>
    {
        opt.ReportApiVersions = true;
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.DefaultApiVersion = new ApiVersion(1, 0);
    }
);
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

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();