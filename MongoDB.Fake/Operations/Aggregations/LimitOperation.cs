namespace MongoDB.Fake.Operations.Aggregations
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Operations;
    using System.Collections.Generic;
    using System.Linq;

    public class LimitOperation<TDocument> : IOperation
    {
        IPipelineStageDefinition _pipelineStageDefinition;
        IBsonSerializer<TDocument> _bsonSerializer;
        ICollection<BsonDocument> _documents;

        public LimitOperation(IPipelineStageDefinition pipelineStageDefinition,
            IBsonSerializer<TDocument> bsonSerializer,
            ICollection<BsonDocument> documents)
        {
            _pipelineStageDefinition = pipelineStageDefinition;
            _bsonSerializer = bsonSerializer;
            _documents = documents;
        }

        public ICollection<BsonDocument> Execute()
        {
            ICollection<BsonDocument> result = new BsonDocumentCollection();
            var serializer = BsonSerializer.SerializerRegistry.GetSerializer(_pipelineStageDefinition.OutputType);
            var renderedStage = _pipelineStageDefinition.Render(serializer, BsonSerializer.SerializerRegistry);
            var limit = renderedStage.Document.GetElement(renderedStage.OperatorName).Value.AsInt32;
            foreach (var document in _documents.Take(limit))
            {
                result.Add(document);
            }
            return result;
        }
    }
}
