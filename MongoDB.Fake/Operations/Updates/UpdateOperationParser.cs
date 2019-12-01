namespace MongoDB.Fake.Operations.Updates
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Operations;
    using System;
    using System.Collections.Generic;


    public class UpdateOperationParser<TDocument> : IOperation
    {
        BsonElement _updateDefintion;
        ICollection<BsonDocument> _documents;
        Dictionary<string, IOperation> _operations;

        public UpdateOperationParser(BsonElement updateDefintion,
            ICollection<BsonDocument> documents)
        {
            _updateDefintion = updateDefintion;
            _documents = documents;
            _operations = CreateOperationParsers();
        }

        private Dictionary<string, IOperation> CreateOperationParsers()
        {
            return new Dictionary<string, IOperation>()
            {
                { Operators.Set, new SetOperation<TDocument>(_updateDefintion, _documents)},
            };
        }

        public ICollection<BsonDocument> Execute()
        {
            IOperation operation;
            if (_operations.TryGetValue(_updateDefintion.Name, out operation))
            {
                return operation.Execute();
            }
            throw new NotImplementedException($"Update operation {_updateDefintion.Name} not supported");
        }
    }
}
