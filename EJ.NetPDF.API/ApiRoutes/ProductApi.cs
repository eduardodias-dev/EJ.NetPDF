using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.ApiRoutes;

public static class ProductApi
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var routeBuilder = endpoints.MapGroup("/products");
        
        routeBuilder.MapPost("/", async (IRepository<Product> repo, Product product) =>
        {
            if (product?.Name is null || product?.Price is null) return Results.UnprocessableEntity();

            await repo.AddAsync(product);
            
            return Results.Created($"/{product.Id}", product);
        });

        routeBuilder.MapGet("/{id:guid}", async (IRepository<Product> repo, Guid id) =>
        {
            var data = await repo.GetByIdAsync(id);

            if (data == null) return Results.NotFound();
            
            return Results.Ok(data);
        });

        routeBuilder.MapPut("/{id:guid}", async (IRepository<Product> repo, Guid id, Product product) =>
        {
            if (product is null) return Results.UnprocessableEntity();
            
            if (id != product?.Id) return Results.BadRequest();
            
            await repo.UpdateAsync(product);
            
            return Results.NoContent();
        });
        
        routeBuilder.MapDelete("/{id:guid}", async (IRepository<Product> repo, Guid id) =>
        {
            var product = await repo.GetByIdAsync(id);
            
            if(product == null) return Results.NotFound();
            
            await repo.DeleteAsync(product);
            
            return Results.NoContent();
        });
        
        routeBuilder.WithTags("Products");
        
        return routeBuilder;
    }
}