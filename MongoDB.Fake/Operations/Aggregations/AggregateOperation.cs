namespace MongoDB.Fake.Operations.Aggregations
{
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Filters;
    using MongoDB.Fake.Filters.Parsers;

    public class AggregateOperation<TDocument, TResult> : IOperation
    {
        private readonly ICollection<BsonDocument> _collection;
        private readonly PipelineDefinition<TDocument, TResult> _pipeline;
        private readonly AggregateOptions _options;

        public AggregateOperation(ICollection<BsonDocument> collection, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null)
        {
            _collection = collection;
            _pipeline = pipeline;
            _options = options;
        }

        public ICollection<BsonDocument> Execute()
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var documents = _collection;
            foreach (var stage in _pipeline.Stages)
            {
                documents = ExecuteAggregateOperation(stage, documentSerializer, documents);
            }
            return documents;
        }

        private ICollection<BsonDocument> ExecuteAggregateOperation(
            IPipelineStageDefinition pipelineStageDefinition,
            IBsonSerializer<TDocument> bsonSerializer,
            ICollection<BsonDocument> documents)
        {
            var processor = new UpdateOperationParser<TDocument>(pipelineStageDefinition, bsonSerializer, documents);
            return processor.Execute();
        }
    }
}
