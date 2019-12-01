using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace MongoDB.Fake.Tests
{
    public class FakeMongoCollectionTests : IClassFixture<MongoCollectionProviderFixture<SimpleTestDocument>>
    {
        private readonly MongoCollectionProviderFixture<SimpleTestDocument> _mongoCollectionProvider;

        public FakeMongoCollectionTests(MongoCollectionProviderFixture<SimpleTestDocument> mongoCollectionProvider)
        {
            _mongoCollectionProvider = mongoCollectionProvider;
        }

        [Fact]
        public async Task FindReturnsDocuments()
        {
            var expectedDocuments = CreateTestData().ToList();

            var collection = CreateMongoCollection(nameof(FindReturnsDocuments));
            var cursor = await collection.FindAsync(d => true);
            var actualDocuments = cursor.ToList();

            actualDocuments.Should().BeEquivalentTo(expectedDocuments);
        }

        [Fact]
        public async Task FindOneAndDeleteReturnsAndDeletesDocument()
        {
            var documentToDelete = CreateTestData().First();
            var expectedRemainedDocuments = CreateTestData().Skip(1).ToList();

            var collection = CreateMongoCollection(nameof(FindOneAndDeleteReturnsAndDeletesDocument));
            var actualDeletedDocument = await collection.FindOneAndDeleteAsync(d => d.Id == documentToDelete.Id);
            var remainedDocuments = collection.Find(d => true).ToList();

            actualDeletedDocument.Should().BeEquivalentTo(documentToDelete);
            remainedDocuments.Should().BeEquivalentTo(expectedRemainedDocuments);
        }

        [Fact]
        public async Task FindOneAndDeleteReturnsNullWhenNothingToDelete()
        {
            var expectedRemainedDocuments = CreateTestData().ToList();

            var collection = CreateMongoCollection(nameof(FindOneAndDeleteReturnsNullWhenNothingToDelete));
            // TODO: Replace filter to "d => false" when $type operator will be implemented
            var actualDeletedDocument = await collection.FindOneAndDeleteAsync(d => d.Id == Guid.Empty);
            var remainedDocuments = collection.Find(d => true).ToList();

            actualDeletedDocument.Should().BeNull();
            remainedDocuments.Should().BeEquivalentTo(expectedRemainedDocuments);
        }

        [Fact]
        public async Task FindOneAndReplaceReturnsOldDocumentAndReplacesIt()
        {
            var documentToReplace = CreateTestData().First();
            var newDocument = CreateTestData().First();
            newDocument.IntField = 4;
            var expectedAllDocuments = CreateTestData().Skip(1)
                .Union(new[] { newDocument })
                .ToList();

            var collection = CreateMongoCollection(nameof(FindOneAndReplaceReturnsOldDocumentAndReplacesIt));
            var actualOldDocument = await collection.FindOneAndReplaceAsync(d => d.Id == documentToReplace.Id, newDocument);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualOldDocument.Should().BeEquivalentTo(documentToReplace);
            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task FindOneAndReplaceReturnsNullWhenNothingToReplace()
        {
            var newDocument = CreateTestData().First();
            newDocument.IntField = 4;
            var expectedAllDocuments = CreateTestData().ToList();

            var collection = CreateMongoCollection(nameof(FindOneAndReplaceReturnsOldDocumentAndReplacesIt));
            // TODO: Replace filter to "d => false" when $type operator will be implemented
            var actualOldDocument = await collection.FindOneAndReplaceAsync(d => d.Id == Guid.Empty, newDocument);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualOldDocument.Should().BeNull();
            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task InsertOneInsertsDocument()
        {
            var newDocument = new SimpleTestDocument { Id = new Guid("00000000-0000-0000-0000-000000000004") };
            var expectedAllDocuments = CreateTestData()
                .Union(new[] { newDocument })
                .ToList();

            var collection = CreateMongoCollection(nameof(InsertOneInsertsDocument));
            await collection.InsertOneAsync(newDocument);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task InsertManyInsertsDocuments()
        {
            var newDocuments = new[]
            {
                new SimpleTestDocument { Id = new Guid("00000000-0000-0000-0000-000000000004") },
                new SimpleTestDocument { Id = new Guid("00000000-0000-0000-0000-000000000005") }
            };
            var expectedAllDocuments = CreateTestData()
                .Union(newDocuments)
                .ToList();

            var collection = CreateMongoCollection(nameof(InsertManyInsertsDocuments));
            await collection.InsertManyAsync(newDocuments);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task CountReturnsTotalCountWithEmptyFilter()
        {
            var expectedCount = CreateTestData().Count();

            var collection = CreateMongoCollection(nameof(CountReturnsTotalCountWithEmptyFilter));
            var actualCount = await collection.CountAsync(d => true);

            actualCount.Should().Be(expectedCount);
        }

        [Fact]
        public async Task CountReturnsActualCountWithFilter()
        {
            var expectedCount = CreateTestData().Count(d => d.IntField == 2);

            var collection = CreateMongoCollection(nameof(CountReturnsActualCountWithFilter));
            var actualCount = await collection.CountAsync(d => d.IntField == 2);

            actualCount.Should().Be(expectedCount);
        }

        [Fact]
        public async Task DeleteOneDeletesOneDocument()
        {
            var documentIdToDelete = new Guid("00000000-0000-0000-0000-000000000002");
            var expectedAllDocuments = CreateTestData()
                .Where(d => d.Id != documentIdToDelete)
                .ToList();
            var expectedResult = new DeleteResult.Acknowledged(1);

            var collection = CreateMongoCollection(nameof(DeleteOneDeletesOneDocument));
            var actualResult = await collection.DeleteOneAsync(d => d.Id == documentIdToDelete);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualResult.Should().BeEquivalentTo(expectedResult);
            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task DeleteOneDoesNothingWithFilter()
        {
            var expectedAllDocuments = CreateTestData().ToList();
            var expectedResult = new DeleteResult.Acknowledged(0);

            var collection = CreateMongoCollection(nameof(DeleteOneDoesNothingWithFilter));
            // TODO: Replace filter to "d => false" when $type operator will be implemented
            var actualResult = await collection.DeleteOneAsync(d => d.Id == Guid.Empty);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualResult.Should().BeEquivalentTo(expectedResult);
            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task DeleteManyDeletesDocuments()
        {
            var expectedAllDocuments = CreateTestData()
              .Where(d => d.IntField != 2)
              .ToList();
            var expectedResult = new DeleteResult.Acknowledged(2);

            var collection = CreateMongoCollection(nameof(DeleteManyDeletesDocuments));
            var actualResult = await collection.DeleteManyAsync(d => d.IntField == 2);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualResult.Should().BeEquivalentTo(expectedResult);
            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task DeleteManyDoesNothingWithFilter()
        {
            var expectedAllDocuments = CreateTestData().ToList();
            var expectedResult = new DeleteResult.Acknowledged(0);

            var collection = CreateMongoCollection(nameof(DeleteManyDoesNothingWithFilter));
            // TODO: Replace filter to "d => false" when $type operator will be implemented
            var actualResult = await collection.DeleteManyAsync(d => d.Id == Guid.Empty);
            var actualAllDocuments = collection.Find(d => true).ToList();

            actualResult.Should().BeEquivalentTo(expectedResult);
            actualAllDocuments.Should().BeEquivalentTo(expectedAllDocuments);
        }

        [Fact]
        public async Task FindSyncWithSkipAndLimit()
        {
            var expectedAllDocuments = CreateTestData().ToList();
            var collection = CreateMongoCollection(nameof(FindSyncWithSkipAndLimit));
            var result = collection.Find(_ => true)
                .Skip(1)
                .Limit(1)
                .ToList();
            var item = Assert.Single(result);
            Assert.Equal(item.Id, expectedAllDocuments[1].Id);
        }

        [Fact]
        public async Task ReplaceOne()
        {
            var allDocuments = CreateTestData().ToList();
            var collection = CreateMongoCollection(nameof(ReplaceOne));
            var itemToReplace = CreateTestData().First();
            itemToReplace.IntField = 999;
            await collection.ReplaceOneAsync(x => x.Id == itemToReplace.Id, itemToReplace);
            var result = Assert.Single(collection.Find(x => x.Id == itemToReplace.Id).ToList());
            Assert.Equal(999, result.IntField);
        }

        [Fact]
        public async Task FindWithArrayAnyFilter()
        {
            var testData = CreateTestData().ToList();
            testData.First().ArrayField = new[]{
                "Hi",
                "Bye",
            };
            var collection = _mongoCollectionProvider.GetCollection(nameof(FindWithArrayAnyFilter), testData);
            Assert.Single(collection.Find(x => x.ArrayField.Any(i => i == "Hi")).ToList());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 0)]
        public async Task AggregateUnwindOperationWithMatch(int intField, int exceptedCount)
        {
            var testData = CreateTestData().ToList();
            var collection = _mongoCollectionProvider.GetCollection(nameof(FindWithArrayAnyFilter), testData);
            collection.InsertOne(new SimpleTestDocument()
            {
                AnotherChildrenDocuments = new[]
                {
                    new AnotherChildDocument()
                    { 
                        StringField = "Hi",
                        Image = new Image { Name = "10" },
                    },
                    new AnotherChildDocument()
                    {
                        StringField = "Bye",
                        Image = new Image { Name = "16" },
                    },
                },
                IntField = intField,
            });

            var currentProjection = new BsonDocumentProjectionDefinition<BsonDocument>(
                BsonDocument.Parse("{ Image: \"$AnotherChildrenDocuments.Image\" }"));

            var result = collection.Aggregate()
                .Match(x => x.IntField == 1)
                .Unwind(x => x.AnotherChildrenDocuments)
                .Match(x => x["AnotherChildrenDocuments.StringField"] == "Hi")
                .Project(currentProjection)
                .As<SimpleTestDocument>()
                .ToList();
            Assert.Equal(exceptedCount, result.Count);
        }

        [Fact]
        public async Task UpdateOne()
        {
            var testData = CreateTestData().ToList();
            var collection = _mongoCollectionProvider.GetCollection(nameof(UpdateOne), new SimpleTestDocument[0]);
            collection.InsertOne(new SimpleTestDocument()
            {
                AnotherChildrenDocuments = new[]
                {
                    new AnotherChildDocument()
                    {
                        StringField = "Hi",
                        Image = new Image { Name = "10" },
                    },
                    new AnotherChildDocument()
                    {
                        StringField = "Hi",
                        Image = new Image { Name = "16" },
                    },
                },
                IntField = 14,
            });

            var filter = Builders<SimpleTestDocument>.Filter.And(
                Builders<SimpleTestDocument>.Filter.Where(x => x.IntField == 14),
                Builders<SimpleTestDocument>.Filter.ElemMatch(x => x.AnotherChildrenDocuments, i => i.StringField == "Hi"));
            var update = Builders<SimpleTestDocument>.Update.Set(
                x => x.AnotherChildrenDocuments.ElementAt(-1).Image,
                new Image { Name = "80" });
            collection.UpdateOne(filter, update);

            var result = Assert.Single(collection.Find(x => x.IntField == 14).ToList());
            Assert.Equal("80", result.AnotherChildrenDocuments[0].Image.Name);
            Assert.Equal("16", result.AnotherChildrenDocuments[1].Image.Name);

        }

        private IMongoCollection<SimpleTestDocument> CreateMongoCollection(string collectionName)
        {
            var testData = CreateTestData();
            return _mongoCollectionProvider.GetCollection(collectionName, testData);
        }

        private IEnumerable<SimpleTestDocument> CreateTestData()
        {
            yield return new SimpleTestDocument
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                IntField = 1
            };
            yield return new SimpleTestDocument
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                IntField = 2
            };
            yield return new SimpleTestDocument
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                IntField = 2
            };
        }
    }
}
