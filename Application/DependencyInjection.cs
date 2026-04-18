using Application.UseCases.Events.Handlers;
using Application.UseCases.Reservations.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register Handlers
            services.AddScoped<ReserveSeatHandler>();
            services.AddScoped<ConfirmPaymentHandler>();
            services.AddScoped<GetAvailableSeatsHandler>();
            services.AddScoped<GetAllSeatsByEventHandler>(); // Nuevo
            services.AddScoped<GetSectorsByEventHandler>(); // Nuevo

            return services;
        }
    }
}
