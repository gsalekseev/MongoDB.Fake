namespace MongoDB.Fake.Operations
{
    using MongoDB.Bson;
    using System.Collections.Generic;

    interface IOperation
    {
        IEnumerable<BsonDocument> Execute();

    }
}
