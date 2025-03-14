using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EJ.NetPDF.API.Models;

public class Entity
{
    [BsonElement("myId")]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
}