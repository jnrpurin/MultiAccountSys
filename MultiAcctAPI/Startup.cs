using Microsoft.OpenApi.Models;
using MultiAcctAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MultiAcctAPI.Services.Interfaces;
using MultiAcctAPI.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddAuthorization();

        services.AddDbContext<AppDBContext>(options =>
            options.UseInMemoryDatabase("InMemoryDb"));

        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IAccountService, AccountService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
       
        // JWT Authentication
        var key = Encoding.ASCII.GetBytes("Ptt8Ri2GTXeuq1BmM4RHS0StgQ4QofEAVsyrHaPOHMA=");
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            // x.Authority = "https://dev-ebfajvxxzdbofrvl.us.auth0.com";
            // x.Audience = "https://dev-ebfajvxxzdbofrvl.us.auth0.com/api/v2/";
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
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
            // c.ExampleFilters();
        });
                // services.AddSwaggerExamplesFromAssemblyOf<Startup>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

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