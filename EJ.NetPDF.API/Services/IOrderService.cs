using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Services;

public interface IOrderService
{
    Task<Order> CreateOrder(CreateOrderModel model);
}