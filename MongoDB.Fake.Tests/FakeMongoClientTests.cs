using FluentAssertions;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Reflection;
using Xunit;

namespace MongoDB.Fake.Tests
{
    public class FakeMongoClientTests
    {
        [Fact]
        public void GetDatabaseReturnsDatabase()
        {
            var client = new FakeMongoClient();
            var database = client.GetDatabase("fake-database");

            database.Should().NotBeNull();
            database.Client.Should().BeSameAs(client);
        }

        [Fact]
        public void DropDatabase()
        {
            var client = new FakeMongoClient();
            client.GetDatabase("fake-database");
            var _databasesProperty = client.GetType().GetField("_databases", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var databases = (ConcurrentDictionary<string, IMongoDatabase>)_databasesProperty.GetValue(client);
            Assert.Single(databases);
            client.DropDatabase("fake-database");
            Assert.Empty(databases);
        }
    }
}