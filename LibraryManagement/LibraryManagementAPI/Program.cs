
using Microsoft.EntityFrameworkCore;
using EFCoreClasses;
using LibraryManagementAPI.Controllers;
using LibraryManagementAPI.Middlewares;

namespace LibraryManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = Environment.GetEnvironmentVariable("LibraryHubDatabase");
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = builder.Configuration.GetValue<string>("LibraryHubDatabase");
            }

            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(connectionString)); // Use the connection string from appsettings.json or default from DbContext


            var app = builder.Build();


            app.UseHttpsRedirection();

            // Configure the HTTP request pipeline.
            // Serve frontend React
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            


            app.UseMiddleware<AdminSafeListMiddleware>(builder.Configuration["AdminSafeList"]);


            app.UseAuthorization();
       
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            //app.MapGet("/", () => "Library Management API is running! Access /swagger to explore the API.");

            app.Run();
        }
    }
}
