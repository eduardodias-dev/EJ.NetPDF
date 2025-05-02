using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Services;

public class OrderFactory : IOrderFactory
{
    private readonly IRepository<Product> _productRepository;
    
    public OrderFactory(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<Order> CreateFromModelAsync(CreateOrderModel model)
    {
        var product = await _productRepository.GetByIdAsync(model.ProductId.GetValueOrDefault());

        if (product == null)
            throw new InvalidOperationException($"Product {model.ProductId} not found.");

        return new Order(model.CustomerId!, model.PaymentType!, product);
    }
}