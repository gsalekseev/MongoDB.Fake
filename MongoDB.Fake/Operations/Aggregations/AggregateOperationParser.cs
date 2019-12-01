namespace MongoDB.Fake.Operations.Aggregations
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Operations;
    using System;
    using System.Collections.Generic;


    public class UpdateOperationParser<TDocument> : IOperation
    {
        IPipelineStageDefinition _pipelineStageDefinition;
        IBsonSerializer<TDocument> _bsonSerializer;
        ICollection<BsonDocument> _documents;
        Dictionary<string, IOperation> _operations;

        public UpdateOperationParser(IPipelineStageDefinition pipelineStageDefinition,
            IBsonSerializer<TDocument> bsonSerializer,
            ICollection<BsonDocument> documents)
        {
            _pipelineStageDefinition = pipelineStageDefinition;
            _bsonSerializer = bsonSerializer;
            _documents = documents;
            _operations = CreateOperationParsers();
        }

        private Dictionary<string, IOperation> CreateOperationParsers()
        {
            return new Dictionary<string, IOperation>()
            {
                { Operators.Unwind, new UnwindOperation<TDocument>(_pipelineStageDefinition, _bsonSerializer, _documents)},
                { Operators.Match, new MatchOperation<TDocument>(_pipelineStageDefinition, _bsonSerializer, _documents)},
                { Operators.Projection, new ProjectOperation<TDocument>(_pipelineStageDefinition, _bsonSerializer, _documents)},
                { Operators.Limit, new LimitOperation<TDocument>(_pipelineStageDefinition, _bsonSerializer, _documents)},
            };
        }

        public ICollection<BsonDocument> Execute()
        {
            IOperation operation;
            if (_operations.TryGetValue(_pipelineStageDefinition.OperatorName, out operation))
            {
                return operation.Execute();
            }
            throw new NotImplementedException($"Aggregate operation {_pipelineStageDefinition.OperatorName} not supported");
        }
    }
}
