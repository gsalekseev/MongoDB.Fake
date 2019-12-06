using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Fake.Tests
{
    public class MongoCollectionProviderFixture<TDocument>
    {
        public IMongoCollection<TDocument> GetCollection(string collectionName, IEnumerable<TDocument> data = null)
        {
            return CreateFakeMongoCollection(collectionName, data);
        }

        private IMongoCollection<TDocument> CreateFakeMongoCollection(string collectionName, IEnumerable<TDocument> data = null)
        {
            var documentCollection = new BsonDocumentCollection();
            if (data != null)
            {
                foreach (var document in data)
                {
                    var bsonDocument = document.ToBsonDocument();
                    documentCollection.Add(bsonDocument);
                }
            }

            var mongoCollection = new FakeMongoCollection<TDocument>(documentCollection);
            return mongoCollection;
        }

        public IMongoCollection<TDocument> CreateRealMongoCollection(string collectionName, IEnumerable<TDocument> data)
        {
            var client = new MongoClient(MongoUrl.Create("mongodb://localhost"));
            var database = client.GetDatabase("fake-database");
            database.DropCollection(collectionName);
            var collection = database.GetCollection<TDocument>(collectionName);
            if(data != null && data.Count() > 0)
                collection.InsertMany(data);
            return collection;
        }
    }
}