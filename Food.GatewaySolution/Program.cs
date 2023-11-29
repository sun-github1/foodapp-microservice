using FluentValidation;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Food.GatewaySolution
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddAuthentication("Bearer")
               .AddJwtBearer("Bearer", options =>
               {
                   options.Authority = "https://localhost:7174/";
                   options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidateAudience = false,
                   };

               });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "food");
                });
            });

            builder.Services.AddOcelot();
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");
            await app.UseOcelot();
            app.Run();
        }
    }
}
