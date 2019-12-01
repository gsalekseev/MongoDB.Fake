namespace MongoDB.Fake.Operations.Aggregations
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Filters.Parsers;
    using MongoDB.Fake.Operations;
    using System.Collections.Generic;
    using System.Linq;

    class MatchOperation<TDocument> : IOperation
    {
        IPipelineStageDefinition _pipelineStageDefinition;
        IBsonSerializer<TDocument> _bsonSerializer;
        ICollection<BsonDocument> _documents;

        public MatchOperation(IPipelineStageDefinition pipelineStageDefinition,
            IBsonSerializer<TDocument> bsonSerializer,
            ICollection<BsonDocument> documents)
        {
            _pipelineStageDefinition = pipelineStageDefinition;
            _bsonSerializer = bsonSerializer;
            _documents = documents;
        }

        public ICollection<BsonDocument> Execute()
        {
            var outputType = _pipelineStageDefinition.OutputType;

            if (outputType == typeof(BsonDocument))
            {
                var serializer = BsonSerializer.SerializerRegistry.GetSerializer<BsonDocument>();
                var renderedStage = _pipelineStageDefinition.Render(serializer, BsonSerializer.SerializerRegistry);
                var matchCondition = renderedStage.Document.GetElement(_pipelineStageDefinition.OperatorName).Value;
                FilterDefinition<BsonDocument> filter = matchCondition.ToString();
                var filterBson = filter.Render(serializer, BsonSerializer.SerializerRegistry);
                var parsedFilter = FilterParser.Instance.Parse(filterBson);
                return new BsonDocumentCollection(_documents.Where(parsedFilter.Filter));
            }
            else
            {
                var renderedStage = _pipelineStageDefinition.Render(_bsonSerializer, BsonSerializer.SerializerRegistry);
                var matchCondition = renderedStage.Document.GetElement(_pipelineStageDefinition.OperatorName).Value;
                FilterDefinition<TDocument> filter = matchCondition.ToString();
                var serializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
                var filterBson = filter.Render(serializer, BsonSerializer.SerializerRegistry);
                var parsedFilter = FilterParser.Instance.Parse(filterBson);
                return new BsonDocumentCollection(_documents.Where(parsedFilter.Filter));
            }
        }
    }
}
