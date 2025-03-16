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

        routeBuilder.MapGet("/orders/{id:guid}", async (IRepository<Order> repo, Guid id) =>
        {
            var result = await repo.GetByIdAsync(id);
            
            return Results.Ok(result);
        });
        
        routeBuilder.WithTags("Orders");
        
        return routeBuilder;
    }
}