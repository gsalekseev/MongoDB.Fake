namespace MongoDB.Fake.Operations
{
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Aggregations;
    using MongoDB.Fake.Filters;
    using MongoDB.Fake.Filters.Parsers;

    public class AggregateOperation<TDocument, TResult> : IOperation
    {
        private readonly ICollection<BsonDocument> _collection;
        private readonly IFilterParser _filterParser;
        private readonly PipelineDefinition<TDocument, TResult> _pipeline;
        private readonly AggregateOptions _options;

        public AggregateOperation(ICollection<BsonDocument> collection, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null)
        {
            _collection = collection;
            _pipeline = pipeline;
            _options = options;
            _filterParser = FilterParser.Instance;
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

        private IFilter Filter(FilterDefinition<TDocument> filterDefinition)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var filterBson = filterDefinition.Render(documentSerializer, BsonSerializer.SerializerRegistry);
            return _filterParser.Parse(filterBson);
        }

        private ICollection<BsonDocument> ExecuteAggregateOperation(
            IPipelineStageDefinition pipelineStageDefinition,
            IBsonSerializer<TDocument> bsonSerializer,
            ICollection<BsonDocument> documents)
        {
            var processor = new AggregateOperationParser<TDocument>(pipelineStageDefinition, bsonSerializer, documents);
            return processor.Execute();
        }
    }
}
