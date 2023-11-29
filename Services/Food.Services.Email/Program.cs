
using Food.Services.Email.Data;
using Food.Services.Email.Extensions;
using Food.Services.Email.Messaging;
using Food.Services.Email.Repository;
using Microsoft.EntityFrameworkCore;

namespace Food.Services.Email
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(options =>
   options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"))
           );

            //IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
            //builder.Services.AddSingleton(mapper);
            //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddScoped<IEmailRepository, EmailRepository>();

            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
            builder.Services.AddSingleton(new EmailRepository(optionBuilder.Options));
            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Food.Services.Email", Version = "v1" });
              
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseAzureServiceBusConsumer();
            app.MapControllers();

            app.Run();
        }
    }
}
