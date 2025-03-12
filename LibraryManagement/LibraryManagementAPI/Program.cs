
using Microsoft.EntityFrameworkCore;
using EFCoreClasses;
using LibraryManagementAPI.Controllers;

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

            var connectionString = builder.Configuration.GetConnectionString("LibraryHubDatabase");

            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(connectionString)); // Use the connection string from appsettings.json or default from DbContext


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // if (app.Environment.IsDevelopment()){}
            
            app.UseSwagger();
            app.UseSwaggerUI();
          

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapGet("/", () => "Library Management API is running! Access /swagger to explore the API.");

            app.Run();
        }
    }
}
