using EJ.NetPDF.API.Data;
using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services;

namespace EJ.NetPDF.API.Extensions;

public static class ServicesConfigurationExtension
{
    public static void AddInternalServices(this IServiceCollection services)
    {
        
        services.AddScoped<IPaymentService, AsaasPaymentService>();
        services.AddScoped<IRepository<Product>, MongoRepository<Product>>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ISubscriptionFactory, SubscriptionFactory>();
        services.AddScoped<IOrderFactory, OrderFactory>();
    }
}