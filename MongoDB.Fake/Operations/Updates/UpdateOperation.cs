namespace MongoDB.Fake.Operations.Updates
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class UpdateOperation<TDocument> : IOperation
    {
        private readonly ICollection<BsonDocument> _itemsToUpdate;
        private readonly BsonDocument _updateDefinition;
        private readonly UpdateOptions _options;

        public UpdateOperation(BsonDocument itemToUpdate, BsonDocument updateDefinition, UpdateOptions options = null)
            : this(ItemToCollection(itemToUpdate), updateDefinition, options)
        { }

        private static ICollection<BsonDocument> ItemToCollection(BsonDocument item)
        {
            var collection = new BsonDocumentCollection();
            collection.Add(item);
            return collection;
        }

        public UpdateOperation(ICollection<BsonDocument> itemsToUpdate, BsonDocument updateDefinition, UpdateOptions options = null)
        {
            _itemsToUpdate = itemsToUpdate;
            _updateDefinition = updateDefinition;
            _options = options;
        }

        public ICollection<BsonDocument> Execute()
        {
            var documents = _itemsToUpdate;
            foreach (var updateOperation in _updateDefinition.Elements)
            {
                documents = ExecuteUpdateOperation(updateOperation, documents);
            }
            return documents;
        }

        private ICollection<BsonDocument> ExecuteUpdateOperation(
            BsonElement updateOpertaion,
            ICollection<BsonDocument> documents)
        {
            var processor = new UpdateOperationParser<TDocument>(updateOpertaion, documents);
            return processor.Execute();
        }
    }
}
