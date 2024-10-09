using AutoMapper;
using GestioneOrdini.Model.Order;
using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using GestioneOrdini.Hubs;
using System;

namespace GestioneOrdini
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Aggiungi questa riga


            // Aggiungi i servizi al container.
            builder.Services.AddControllers();

            // Aggiungi SignalR per la comunicazione in tempo reale
            builder.Services.AddSignalR();

            // Configura il DbContext con SQL Server
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

            // Configurazione di CORS per abilitare il frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
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
                        ClockSkew = TimeSpan.Zero,  // Riduce la tolleranza per la validità del token
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Aggiungi autorizzazione
            builder.Services.AddAuthorization();

            // Registrazione di AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configurazione per la registrazione dei log
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole(); // Logs to the console
                loggingBuilder.AddDebug();   // Logs to the debug window
            });

            // Configurazione Swagger/OpenAPI per la documentazione API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestioneOrdini API", Version = "v1" });

                // Configura l'autenticazione JWT per Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Inserisci il token JWT nel formato: Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Configura il middleware per servire file statici dalla cartella "FileUpload"
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "FileUpload")),
                RequestPath = "/uploads"
            });

            // Configura la pipeline delle richieste HTTP.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GestioneOrdini API v1"));
            }
            else
            {
                // Gestione globale degli errori in produzione
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseRouting();

            // Abilita CORS
            app.UseCors("AllowFrontend");

            // Redirect su HTTPS e gestione autenticazione/ autorizzazione
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Configura gli endpoint
            app.MapControllers();

            // Aggiungi il supporto per SignalR e gli hub
            app.MapHub<OrderHub>("/Hubs/orderHub");

            app.Run();
        }
    }
}
