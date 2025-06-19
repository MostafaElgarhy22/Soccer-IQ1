using Microsoft.EntityFrameworkCore;
using Soccer_IQ.Data;
using Soccer_IQ.Models;
using Soccer_IQ.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Soccer_IQ.Repository;
using Microsoft.AspNetCore.Identity;

namespace Soccer_IQ
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default"))
                    };
                });

            // ❌ Commented out database and identity services for testing without SQL Server

            
            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IRepository<Player>, PlayerRepository>();
            builder.Services.AddScoped<IRepository<Club>, ClubRepository>();
            builder.Services.AddScoped<IRepository<PLayerStat>, PlayerStatRepository>();
            builder.Services.AddScoped<IRepository<LeagueStanding>, LeagueStandingRepository>();
            builder.Services.AddScoped<IRepository<ApplicationUser>, ApplicationUserRepository>();
            
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<StandingsSyncService>();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            // ❌ Removed to avoid Render HTTPS redirect issue
            // app.UseHttpsRedirection();

            app.MapControllers();

            // ❌ Commented out role seeding (requires DB)
            /*
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
            }
            */

            app.Run();
        }
    }
}