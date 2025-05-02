using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Services;

public interface IOrderFactory
{
    Task<Order> CreateFromModelAsync(CreateOrderModel model);
}