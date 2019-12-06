using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MongoDB.Fake.Tests
{
    public class SimpleTestDocument
    {
        public class ChildDocument
        {
            public string StringField { get; set; }
            public int IntField { get; set; }
        }

        public Guid Id { get; set; }
        public string StringField { get; set; }
        public int IntField { get; set; }
        public ChildDocument Child { get; set; }
        public ChildDocument[] Children { get; set; }
        public string[] ArrayField { get; set; }
        public Image Image { get; set; }
        public AnotherChildDocument AnotherChildDocument { get; set; }
        public AnotherChildDocument[] AnotherChildrenDocuments { get; set; }
    }

    public class Image
    {
        public string Name { get; set; }
    }

    public class AnotherChildDocument
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
        public Image Image { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DocumentWithBsonId
    {
        [BsonId]
        public string Id { get; set; }
    }
}