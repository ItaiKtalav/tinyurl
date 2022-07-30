using MongoDB.Driver;
using TinyUrl.Models;

namespace TinyUrl.Services
{
    public interface IShortenerService
    {
        /// <summary>
        /// Shortens the given URL to a 6 digit key
        /// </summary>
        /// <param name="url">The URL that the redirect URL will redirect to</param>
        /// <returns>The 6 digit key used for the small redirect URL</returns>
        Task<string> Shorten(string url);

        /// <summary>
        /// Get's the url from the given 6 digit key
        /// </summary>
        /// <param name="key">The unique 6 digit key</param>
        /// <returns>The url that was shortened or null if it doesn't exist</returns>
        Task<string?> Get(string key);
    }

    public class ShortenerService : IShortenerService
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<ShortUrl> _collection;

        public ShortenerService(IMongoClient mongoClient)
        {
            _db = mongoClient.GetDatabase("local");
            _collection = _db.GetCollection<ShortUrl>("Tinyurl");
        }

        public async Task<string> Shorten(string url)
        {
            // I know I should probably be using hashing
            // I didn't in order to keep things simple and less room for error
            // Not to mention that it wassn't in the criteria

            var shortenedModel = await _collection.Find(m => m.Url == url).FirstOrDefaultAsync();
            
            if (shortenedModel == null)
            {
                var key = await GenerateKey();

                shortenedModel = new ShortUrl
                {
                    Url = url,
                    Key = key
                };

                await _collection.InsertOneAsync(shortenedModel);
            }
            
            return shortenedModel.Key;
        }

        public async Task<string?> Get(string key)
        {
            var shortenedModel = await _collection.Find(m => m.Key == key).FirstOrDefaultAsync();
            return shortenedModel?.Url;
        }

        private async Task<string> GenerateKey()
        {
            string key;
            do
            {
                key = KeyGenerator.Generate(6);
            } while (await _collection.Find(m => m.Key == key).CountDocumentsAsync() > 0);

            return key;
        }
    }
}
