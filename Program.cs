
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;

namespace SyncFoodApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var context = new SyncFoodContext();

            // créé le fichier de db si il n'existe pas + effectue les migrations si besoin
            if (context.Database.EnsureCreated())
            {
                // créé un compte admin par défaut (seulement dans le cas où on a eu besoin de créé le fichier de db
                User defaultAdminAccount = new User
                {
                    UserName = "Admin",
                    Discriminator = "#0000",
                    Email = "admin@admin",
                    Role = Role.ADMIN,
                    Password = BCrypt.Net.BCrypt.HashPassword("adminadmin"),
                    CreationDate = DateTime.Now
                };

                context.Users.Add(defaultAdminAccount);
                context.SaveChanges();
            }

            // Add services to the container.

            builder.Services.AddControllers();


            builder.Services.AddDbContext<SyncFoodContext>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Ajoute le champ token à la gui de swagger
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "standard Authorisation header using Bearer scheme (\"bearer [token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // Ajoute le service d'auth pour les token jwt
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // à gérer côté serveur d'hébergement (NGINX)
            // app.UseHttpsRedirection();


            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}