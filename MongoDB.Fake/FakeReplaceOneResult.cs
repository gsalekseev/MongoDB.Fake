namespace MongoDB.Fake
{
    using MongoDB.Bson;
    using MongoDB.Driver;

    class FakeReplaceOneResult : ReplaceOneResult
    {
        readonly long _matchedCount;
        readonly long? _modifiedCount;
        readonly BsonValue _upsertedId;
        public FakeReplaceOneResult(
            long MatchedCount,
            BsonValue UpsertedId = null,
            long? ModifiedCount = null)
        {
            _upsertedId = UpsertedId;
            _matchedCount = MatchedCount;
            _modifiedCount = ModifiedCount;
        }

        public override bool IsAcknowledged => true;

        public override bool IsModifiedCountAvailable => _modifiedCount != null;

        public override long MatchedCount => _matchedCount;

        public override long ModifiedCount => _modifiedCount ?? 0;

        public override BsonValue UpsertedId => _upsertedId;
    }
}
