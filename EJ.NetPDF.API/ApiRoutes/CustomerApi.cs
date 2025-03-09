using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services;

namespace EJ.NetPDF.API.ApiRoutes;

public static class CustomerApi
{
    public static RouteGroupBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var routeBuilder = app.MapGroup("/customers");
        
        routeBuilder.MapGet("/", async (IPaymentService paymentService) => {
                var result = await paymentService.GetCustomers();
    
                return Results.Ok(result);
            });
        
        routeBuilder.MapGet("/{id}", async (IPaymentService paymentService, string id) =>
        {
            var result = await paymentService.GetCustomer(id);
    
            return Results.Ok(result);
        });

        routeBuilder.MapPost("/", async (IPaymentService paymentService, AddCustomerModel customer) =>
        {
            var result = await paymentService.CreateCustomer(customer);
    
            return Results.Created($"/{result.Id}", result);
        });

        routeBuilder.MapPut("/{id}", async (IPaymentService paymentService, string id, UpdateCustomerModel customer) =>
        {
            _ = await paymentService.UpdateCustomer(customer);
        
            return Results.NoContent(); 
        });

        routeBuilder.MapDelete("/{id}", async (IPaymentService paymentService, string id) =>
        {
            bool result = await paymentService.DeleteCustomer(id);
    
            return result ? Results.NoContent() : Results.NotFound();
        });

        routeBuilder.WithTags("Customers");

        return routeBuilder;
    }
}