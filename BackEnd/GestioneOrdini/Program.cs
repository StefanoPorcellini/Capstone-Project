using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GestioneOrdini
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Aggiungi i servizi al container.
            builder.Services.AddControllers();

            // Aggiungi DbContext con il SQL Server provider
            builder.Services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbGO")));

            // Registrazione dei servizi con le loro implementazioni
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ILaserPriceListService, LaserPriceListService>();

            // Registrazione servizio generico
            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

            // Aggiungi l'accesso al contesto HTTP per l'uso nei servizi
            builder.Services.AddHttpContextAccessor();

            // Configurazione di CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Origine frontend
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // Configurazione dell'autenticazione JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Aggiungi autorizzazione
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configura la pipeline delle richieste HTTP.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Configurazione della gestione degli errori globali per la produzione
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // Abilita CORS
            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            // Configura la sicurezza, ad esempio, autenticazione e autorizzazione
            app.UseAuthentication(); // Se usi autenticazione
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
