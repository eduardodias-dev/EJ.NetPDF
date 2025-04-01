using System.Diagnostics.CodeAnalysis;
using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EJ.NetPDF.API.ApiRoutes;

public static class ProductApi
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var routeBuilder = endpoints.MapGroup("/products");
        
        routeBuilder.MapPost("/", async (IRepository<Product> repo, ProductModel productModel) =>
        {
            if (productModel?.Name is null || productModel?.Price is null) return Results.UnprocessableEntity();

            var product = new Product()
            {
                Name = productModel.Name,
                Cycle = productModel.Cycle,
                Price = productModel.Price,
                Description = productModel.Description
            };
            
            await repo.AddAsync(product);
            return Results.Created($"/{product.Id}", product);
        });

        routeBuilder.MapGet("", async (IRepository<Product> repository, 
            [FromQuery] int offset = 0, [FromQuery] int limit = 10) =>
        {
            var data = await repository.GetAllAsync(offset, limit);
            
            var result = data.Select(x => new ProductModel(x.Id, x.Name!, x.Description!, x.Price, x.Cycle!));
            
            return Results.Ok(result);
        });
        
        routeBuilder.MapGet("/{id:guid}", async (IRepository<Product> repo, Guid id) =>
        {
            var data = await repo.GetByIdAsync(id);

            if (data == null) return Results.NotFound();

            var productModel = new ProductModel(data.Id, data.Name!, data.Description!, data.Price, data.Cycle!);
            
            return Results.Ok(productModel);
        });

        routeBuilder.MapPut("/{id:guid}", async (IRepository<Product> repo, Guid id, ProductModel productModel) =>
        {
            if (productModel is null) return Results.UnprocessableEntity();
            
            if (id != productModel?.Id) return Results.BadRequest();
            
            var product = new Product()
            {
                Id = productModel.Id.GetValueOrDefault(),
                Name = productModel.Name,
                Cycle = productModel.Cycle,
                Price = productModel.Price,
                Description = productModel.Description
            };
            
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

public class PaginationFilter
{
    public int Offset { get; }
    public int Limit { get; }

    public PaginationFilter()
    {
        Offset = 1;
        Limit = 10;
    }
}