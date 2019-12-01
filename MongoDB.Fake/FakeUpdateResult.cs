namespace MongoDB.Fake
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;

    public class FakeUpdateResult : UpdateResult
    {
        readonly long _matchedCount;
        readonly long? _updatedCount;
        readonly BsonValue _upsertedId;
        public FakeUpdateResult(
            long MatchedCount,
            BsonValue UpsertedId = null,
            long? UpdatedCount = null)
        {
            _upsertedId = UpsertedId;
            _matchedCount = MatchedCount;
            _updatedCount = UpdatedCount;
        }

        public override bool IsAcknowledged => true;

        public override bool IsModifiedCountAvailable => _updatedCount != null;

        public override long MatchedCount => _matchedCount;

        public override long ModifiedCount => _updatedCount ?? 0;

        public override BsonValue UpsertedId => _upsertedId;
    }
}
