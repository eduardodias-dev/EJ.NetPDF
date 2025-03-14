using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EJ.NetPDF.API.Data;

public class MongoRepository<T> : IRepository<T> where T : Entity
{
    private readonly string _collectionName;
    private readonly IConfiguration _configuration;
    
    public MongoRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        
        _collectionName = string.Format("{0}s", typeof(T).Name.ToLower());
    }

    private IMongoDatabase GetDatabase()
    {
        var client = new MongoClient(_configuration["MongoDB:ConnectionString"]);
        
        return client.GetDatabase(_configuration["MongoDB:Database"]);
    }
    
    public async Task<T> GetByIdAsync(Guid id)
    {
        var collection = GetCollection();
        
        var data = await collection.FindAsync<T>(Builders<T>.Filter.Eq("_id", id.ToString()));
        
        return data.FirstOrDefault();
    }

    private IMongoCollection<T> GetCollection()
    {
        var db = GetDatabase();

        return db.GetCollection<T>(_collectionName);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var collection = GetCollection();
        
        var data = collection.FindAsync<T>(Builders<T>.Filter.Empty);
        
        return await data.Result.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        var collection = GetCollection();
        
        entity.Id = Guid.NewGuid();

        await collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var collection = GetCollection();
        
        await collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", entity.Id.ToString()), entity);
    }

    public async Task DeleteAsync(T entity)
    {
        var collection = GetCollection();
        
        await collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", entity.Id.ToString()));
    }
}