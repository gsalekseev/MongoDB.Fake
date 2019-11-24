namespace MongoDB.Fake.Aggregations
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Filters.Parsers;
    using MongoDB.Fake.Operations;
    using System;
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
            var renderedStage = _pipelineStageDefinition.Render(_bsonSerializer, BsonSerializer.SerializerRegistry);
            var matchCondition = renderedStage.Document.GetElement("$match").Value;
            FilterDefinition<TDocument> filter = matchCondition.ToString();
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var filterBson = filter.Render(documentSerializer, BsonSerializer.SerializerRegistry);
            var parsedFilter = FilterParser.Instance.Parse(filterBson);
            return new BsonDocumentCollection(_documents.Where(parsedFilter.Filter));
        }
    }
}
