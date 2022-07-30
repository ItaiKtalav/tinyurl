using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrl.Models;

[BsonIgnoreExtraElements]
public class ShortUrl
{
    public string Url { get; init; } 
    public string Key { get; init; } 
}

