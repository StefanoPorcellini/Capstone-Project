using GestioneOrdini.Context;
using GestioneOrdini.Interface;
using GestioneOrdini.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            // Registrazione IUserService con la sua implementazione UserService
            builder.Services.AddScoped<IUserService, UserService>();

            // Registrazione ICustomerService con la sua implementazione CustomerService
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            // Registrazione IOrderService con la sua implementazione OrderService (se applicabile)
            //builder.Services.AddScoped<IOrderService, OrderService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configurazione della gestione degli errori globali (opzionale)
            builder.Services.AddExceptionHandler(options =>
            {
                options.ExceptionHandler = context =>
                {
                    // Configura la gestione degli errori
                    // Ad esempio, restituisci una risposta JSON con informazioni sull'errore
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"An error occurred.\"}");
                };
            });

            var app = builder.Build();

            // Configura la pipeline delle richieste HTTP.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Configura la sicurezza, ad esempio, autenticazione e autorizzazione
            // app.UseAuthentication();
            // app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
