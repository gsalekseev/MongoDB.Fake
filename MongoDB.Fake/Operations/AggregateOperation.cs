namespace MongoDB.Fake.Operations
{
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;

    public class AggregateOperation<TDocument, TResult> : IOperation
    {
        private readonly IEnumerable<BsonDocument> _collection;
        private readonly PipelineDefinition<TDocument, TResult> _pipeline;
        private readonly AggregateOptions _options;

        public AggregateOperation(IEnumerable<BsonDocument> collection, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null)
        {
            _collection = collection;
            _pipeline = pipeline;
            _options = options;
        }

        public IEnumerable<BsonDocument> Execute()
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var documents = new List<BsonDocument>(_collection);
            foreach (var stage in _pipeline.Stages)
            {
                documents = ExecuteOneOperation(stage, documentSerializer, documents);
            }
            return documents;
        }

        private List<BsonDocument> ExecuteOneOperation(
            IPipelineStageDefinition pipelineStageDefinition,
            IBsonSerializer<TDocument> bsonSerializer,
            List<BsonDocument> documents)
        {
            if (pipelineStageDefinition.OperatorName == "$unwind")
            {
                List<BsonDocument> result = new List<BsonDocument>();
                var renderedStage = pipelineStageDefinition.Render(bsonSerializer, BsonSerializer.SerializerRegistry);
                var fieldForUnwind = renderedStage.Document.GetElement("$unwind").Value.AsString.Remove(0, 1);
                foreach (var document in documents)
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
            return documents;
        }
    }
}
