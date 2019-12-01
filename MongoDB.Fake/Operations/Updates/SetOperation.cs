namespace MongoDB.Fake.Operations.Updates
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MongoDB.Bson;
    using MongoDB.Fake.Extensions;

    public class SetOperation<TDocument> : IOperation
    {
        BsonElement _updateDefintion;
        ICollection<BsonDocument> _documents;

        public SetOperation(BsonElement updateDefintion,
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
                foreach (var updateProperty in _updateDefintion.Value.AsBsonDocument.Elements)
                {
                    var updatedDocument = document.Update(updateProperty.Name, updateProperty.Value);
                    result.Add(updatedDocument);
                }
            }

            return result;
        }
    }
}
