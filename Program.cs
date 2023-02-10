
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;

namespace SyncFoodApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var context = new SyncFoodContext();

            // créé le fichier de db si il n'existe pas
            context.Database.EnsureCreated();

            // Add services to the container.

            builder.Services.AddControllers();

            
            builder.Services.AddDbContext<SyncFoodContext>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            
        }
    }
}