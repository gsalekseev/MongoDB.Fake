namespace MongoDB.Fake.Aggregations
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Fake.Operations;
    using System.Collections.Generic;

    public class UnwindOperation<TDocument> : IOperation
    {
        IPipelineStageDefinition _pipelineStageDefinition;
        IBsonSerializer<TDocument> _bsonSerializer;
        ICollection<BsonDocument> _documents;

        public UnwindOperation(IPipelineStageDefinition pipelineStageDefinition,
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
            var renderedStage = _pipelineStageDefinition.Render(_bsonSerializer, BsonSerializer.SerializerRegistry);
            var fieldForUnwind = renderedStage.Document.GetElement("$unwind").Value.AsString.Remove(0, 1);
            foreach (var document in _documents)
            {
                if (document.AsBsonDocument.Contains(fieldForUnwind) && document.AsBsonDocument[fieldForUnwind].IsBsonArray)
                {
                    for (int i = 0; i < document.AsBsonDocument[fieldForUnwind].AsBsonArray.Count; i++)
                    {
                        var unwindedDocument = new BsonDocument(document.AsBsonDocument);
                        unwindedDocument[fieldForUnwind] = unwindedDocument[fieldForUnwind][i];
                        result.Add(unwindedDocument.AsBsonDocument);
                    }
                }
            }
            return result;
        }
    }
}
