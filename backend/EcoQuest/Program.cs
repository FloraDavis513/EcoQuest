using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcoQuest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<eco_questContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidIssuer = AuthenticationOptions.ISSUER, ValidateAudience = true,
                    ValidAudience = AuthenticationOptions.AUDIENCE, ValidateLifetime = true, IssuerSigningKey = AuthenticationOptions.GetSymmetricSecurityKey(), ValidateIssuerSigningKey = true };
            });
            builder.Services.AddAuthorization();
            //builder.Services.AddCors();

            WebApplication app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            /*
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });
            */

            ApplicationService applicationService = new ApplicationService();
            ApplicationController applicationController = new ApplicationController(app, applicationService);

            applicationController.Map();

            app.MapGet("/test/api", () =>
            {
                Console.WriteLine("==========/test/api==========");

                return Results.Json(new { TestField1 = "TestValue1", TestField2 = "TestValue2" });
            });

            app.Run();
        }
    }
}