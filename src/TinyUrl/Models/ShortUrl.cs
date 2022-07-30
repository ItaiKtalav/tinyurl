using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrl.Models
{
    [BsonIgnoreExtraElements]
    public class ShortUrl
    {
        public string Url { get; set; } = "";
        public string Key { get; set; } = "";
    }
}
