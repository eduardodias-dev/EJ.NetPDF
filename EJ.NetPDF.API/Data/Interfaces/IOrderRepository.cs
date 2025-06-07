using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Data.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetOrdersByCustomerId(string customerId);
}