using Food.Services.IdentityServer.Data;
using Food.Services.IdentityServer.Initializer;
using Food.Services.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Food.Services.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"))
            );
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
           var identityServerBuilder= builder.Services.AddIdentityServer(options => { 
                options.Events.RaiseErrorEvents= true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(SD.IdentityResources)
                .AddInMemoryApiScopes(SD.ApiScopes)
                .AddInMemoryClients(SD.Clients)
                .AddAspNetIdentity<ApplicationUser>();
            identityServerBuilder.AddDeveloperSigningCredential();

            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            DbInitializer.Initialize();
            app.MapRazorPages();

            app.Run();
        }
    }
}
