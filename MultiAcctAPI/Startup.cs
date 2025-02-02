using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MultiAcctAPI.Services;
using MultiAcctAPI.Interfaces;
using MultiAcctAPI.Data;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddDbContext<AppDBContext>(options =>
            options.UseInMemoryDatabase("InMemoryDb"));

        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<ITransactionService, TransactionService>();

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
       
        // JWT Authentication
        var jwtSettings = _configuration.GetSection("JWTSecrets");
        var jwtSecret = jwtSettings["JwtSecret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            throw new ArgumentNullException("JWT secret is not configured properly.");
        }

        var key = Encoding.ASCII.GetBytes(jwtSecret);
        var issuer = jwtSettings["JwtIssuer"];
        var audience = jwtSettings["JwtAudience"];
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidIssuer = issuer, 
                ValidateIssuer = true,
                ValidAudience = audience, 
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)

            };
        });

        services.AddAuthorization();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MultiAcctAPI", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    }, Array.Empty<string>()
                }
            });
            c.EnableAnnotations();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MultiAcctAPI v1");
            c.RoutePrefix = string.Empty;
        });
    }
}