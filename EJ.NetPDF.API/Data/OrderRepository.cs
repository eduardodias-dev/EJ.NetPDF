using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using MongoDB.Driver;

namespace EJ.NetPDF.API.Data;

public class OrderRepository : MongoRepository<Order>, IOrderRepository
{
    public OrderRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<List<Order>> GetOrdersByCustomerId(string customerId)
    {
        var collection = GetCollection();
    
        var data = await collection.FindAsync<Order>(Builders<Order>.Filter.Eq("CustomerId", customerId));
    
        return await data.ToListAsync();
    }
}