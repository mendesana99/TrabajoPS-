using Application.UseCases.Events.Handlers;
using Application.UseCases.Reservations.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<GetEventsHandler>();
            services.AddScoped<GetSeatsByEventHandler>();
            services.AddScoped<ReserveSeatHandler>();
            services.AddScoped<ConfirmPaymentHandler>();

            return services;
        }
    }
}
