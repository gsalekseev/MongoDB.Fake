namespace MongoDB.Fake.Extensions
{
    using MongoDB.Bson;
    using System.Linq;

    public static class BsonDocumentExtensions
    {

        public static BsonValue Find(this BsonDocument bsonDocument, string name)
        {
            return bsonDocument.AsBsonValue.Find(name);
        }

        public static BsonDocument Update(this BsonDocument bsonDocument, string name, BsonValue value)
        {
            return bsonDocument.AsBsonValue.Update(name, value).AsBsonDocument;
        }

        public static bool Exists(this BsonDocument bsonDocument, string name)
        {
            return bsonDocument.Find(name) != BsonNull.Value;
        }
    }
}
