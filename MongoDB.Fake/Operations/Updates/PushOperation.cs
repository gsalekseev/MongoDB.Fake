namespace MongoDB.Fake.Operations.Updates
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Fake.Extensions;

    public class PushOperation<TDocument> : IOperation
    {
        BsonElement _updateDefintion;
        ICollection<BsonDocument> _documents;

        public PushOperation(BsonElement updateDefintion,
            ICollection<BsonDocument> documents)
        {
            _updateDefintion = updateDefintion;
            _documents = documents;
        }

        public ICollection<BsonDocument> Execute()
        {
            ICollection<BsonDocument> result = new BsonDocumentCollection();

            foreach (var document in _documents)
            {
                var tempDocument = document;
                foreach (var updateProperty in _updateDefintion.Value.AsBsonDocument.Elements)
                {
                    var propertyName = updateProperty.Name;
                    var updateValue = updateProperty.Value;

                    BsonValue newValue = new BsonArray();
                    if (document.TryGetElement(propertyName, out BsonElement oldValue))
                    {
                        newValue = oldValue.Value;
                    }

                    newValue.AsBsonArray.Add(updateValue);
                    tempDocument[updateProperty.Name] = newValue;
                }
                result.Add(tempDocument);
            }

            return result;
        }
    }
}
