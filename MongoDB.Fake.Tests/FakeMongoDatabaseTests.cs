using FluentAssertions;
using MongoDB.Bson;
using System.Collections.Concurrent;
using System.Reflection;
using Xunit;

namespace MongoDB.Fake.Tests
{
    public class FakeMongoDatabaseTests
    {
        [Fact]
        public void GetCollectionReturnsCollection()
        {
            var database = new FakeMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("fake-collection");
            collection.Should().NotBeNull();
            collection.Database.Should().BeSameAs(database);
        }

        [Fact]
        public void DropCollection()
        {
            var database = new FakeMongoDatabase();
            database.GetCollection<BsonDocument>("fake-collection");
            var _collectionsProperty = database.GetType().GetField("_collections", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var _collections = (ConcurrentDictionary<string, BsonDocumentCollection>)_collectionsProperty.GetValue(database);
            Assert.Single(_collections);
            database.DropCollection("fake-collection");
            Assert.Empty(_collections);
        }
    }
}