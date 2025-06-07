using System.Diagnostics.CodeAnalysis;
using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EJ.NetPDF.API.ApiRoutes;

public static class OrderApi
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var routeBuilder = endpoints.MapGroup("/orders");

        routeBuilder.MapPost("/", async (IOrderService service, CreateOrderModel orderModel) =>
        {
            var result = await service.CreateOrder(orderModel);

            return Results.Created($"/{result.Id}", result);
        });

        routeBuilder.MapGet("/{id:guid}", async (IOrderService service, Guid id) =>
        {
            var result = await service.GetOrderById(id);
            
            return Results.Ok(result);
        });

        routeBuilder.MapGet("/customer/{customerId}", async (IOrderService service, string customerId) =>
        {
            var result = await service.GetOrdersByCustomerId(customerId);
            
            return Results.Ok(result);
        });
        
        routeBuilder.WithTags("Orders");
        
        return routeBuilder;
    }
}