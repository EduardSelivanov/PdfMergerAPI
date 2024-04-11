
using Microsoft.EntityFrameworkCore;
using PdfMergerAPI.DBContexts;
using PdfMergerAPI.Repository;
using PdfMergerAPI.Services;
using Serilog;

namespace PdfMergerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<PdfMergerDBContext>(opt => opt.UseNpgsql(
                builder.Configuration.GetConnectionString("ConnectionStringMerger")));
            builder.Services.AddSingleton<PathsService>();

            //var LoggerToTXT=new LoggerConfiguration()
            //    .WriteTo.File("Logs/history.txt", rollingInterval:RollingInterval.Day,fileSizeLimitBytes:null)
            //    .MinimumLevel.Error()
            //    .CreateLogger();

            //builder.Logging.ClearProviders();
            //builder.Logging.AddSerilog(LoggerToTXT);

            builder.Services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(provider => new CustomLogger("Logs/history.txt"));

            

           


            builder.Services.AddScoped<IPdfMerger, PdfMerger>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}