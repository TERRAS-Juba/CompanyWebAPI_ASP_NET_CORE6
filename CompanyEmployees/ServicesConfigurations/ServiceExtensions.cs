using System.Reflection;
using System.Text;
using AspNetCoreRateLimit;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Formatters;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Npgsql;
using Repository;
using Repository.DataShaping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CompanyEmployees.ServicesConfigurations;

public static class ServiceExtensions
{
    public static void ConfigureDIForAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void ConfigureDIForRepositoryBase(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    }

    public static void ConfigurePostgresDatabaseConnection(this WebApplicationBuilder builder, string connectionString,
        string migrationAssembly)
    {
        var connectionBuilder = new NpgsqlConnectionStringBuilder();
        connectionBuilder.ConnectionString = connectionString;
        builder.Services.AddDbContext<RepositoryContext>(o =>
            o.UseNpgsql(connectionBuilder.ConnectionString, b => b.MigrationsAssembly(migrationAssembly)));
    }

    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Host.UseNLog();
    }

    public static void ConfigureFilters(this IServiceCollection services)
    {
        services.AddScoped<ValidationFilterAttribute>();
        services.AddScoped<ValidateCompanyExistsAttribute>();
    }

    public static void ConfigureDIForDataShaper(this IServiceCollection services)
    {
        services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
        services.AddScoped<IDataShaper<CompanyDto>, DataShaper<CompanyDto>>();
        services.AddScoped<IDataShaper<CompanyJoinEmployeeDto>, DataShaper<CompanyJoinEmployeeDto>>();
    }

    public static void ConfigureAPIVersionning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
            }
        );
        // Replace error 400 with 422 when validation rules of incoming dto is not valide
        // 400 => Bad request
        // 422 => Unprocessable Entity
        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
    }

    public static void ConfigureCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy("CorsPolicy", build => build.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
    }

    public static void ConfigureControllersBehaviour(this IServiceCollection services)
    {
        services.AddControllers(config =>
                {
                    // Content Negotiation
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                }
                //Added support for XML format for requests 
            ).AddXmlDataContractSerializerFormatters()
            // Added support for patch requests
            .AddNewtonsoftJson();
    }

    public static void ConfigureCustomOutputFormaters(this IServiceCollection services)
    {
        services.AddMvc(config => config.OutputFormatters.Add(new CsvOutputFormatter()));
    }

    public static void ConfigureResponseCaching(this IServiceCollection services)
    {
        services.AddResponseCaching();
    }

    public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
    {
        services.AddHttpCacheHeaders((expirationOpt) =>
            {
                expirationOpt.MaxAge = 20;
                expirationOpt.CacheLocation = CacheLocation.Public;
            },
            (validationOpt) => { validationOpt.MustRevalidate = true; });
    }

    public static void ConfigureRateLimitingOptions(this IServiceCollection services)
    {
        var rateLimitRules = new List<RateLimitRule>()
        {
            new()
            {
                Endpoint = "*",
                Limit = 100,
                Period = "5m"
            }
        };
        services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentityCore<User>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 10;
            o.User.RequireUniqueEmail = true;
        });
        builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole),
            builder.Services);
        builder.AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                ValidAudience = jwtSettings.GetSection("validAudience").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "My Company API",
                Description = "An ASP.NET Core Web API for managing Companies with their employees",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Example Contact",
                    Url = new Uri("https://example.com/contact")
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license")
                }
            });
            opt.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = "My Company API",
                Description = "An ASP.NET Core Web API for managing Companies with their employees version 2",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "TERRAS Juba",
                    Email = "jubaterras600@gmail.com",
                    Url = new Uri("https://example.com/contact")
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license")
                }
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });
        });
    }
}