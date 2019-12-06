namespace MongoDB.Fake.Operations.Aggregations
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Extensions;
    using MongoDB.Fake.Operations;
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectOperation<TDocument> : IOperation
    {
        IPipelineStageDefinition _pipelineStageDefinition;
        IBsonSerializer<TDocument> _bsonSerializer;
        ICollection<BsonDocument> _documents;

        public ProjectOperation(IPipelineStageDefinition pipelineStageDefinition,
            IBsonSerializer<TDocument> bsonSerializer,
            ICollection<BsonDocument> documents)
        {
            _pipelineStageDefinition = pipelineStageDefinition;
            _bsonSerializer = bsonSerializer;
            _documents = documents;
        }

        public ICollection<BsonDocument> Execute()
        {
            var serializer = BsonSerializer.SerializerRegistry.GetSerializer<BsonDocument>();
            var renderedStage = _pipelineStageDefinition.Render(serializer, BsonSerializer.SerializerRegistry);
            var projections = renderedStage.Document.GetElement(renderedStage.OperatorName).Value.AsBsonDocument;

            ICollection<BsonDocument> result = new BsonDocumentCollection();
            foreach (var document in _documents)
            {
                BsonDocument projectedDocument = new BsonDocument();
                for (int i = 0; i < projections.Names.Count(); i++)
                {
                    var whatToTake = projections.Values.ElementAt(i);
                    if (whatToTake.IsString && whatToTake.AsString.StartsWith("$"))
                    {
                        var projectedValue = document.Find(whatToTake.AsString.Remove(0, 1));
                        projectedDocument.Add(projections.Names.ElementAt(i), projectedValue);
                    }
                    else
                    {
                        projectedDocument.Add(projections.Names.ElementAt(i), whatToTake.AsBsonValue);
                    }
                }
                result.Add(projectedDocument);
            }

            return result;
        }
    }
}
