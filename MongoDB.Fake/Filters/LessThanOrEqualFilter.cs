namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;

    internal class LessThanOrEqualFilter : IFilter
    {
        private readonly BsonValue _specifiedValue;

        public LessThanOrEqualFilter(BsonValue specifiedValue)
        {
            _specifiedValue = specifiedValue;
        }

        public bool Filter(BsonValue value)
        {
            return value.CompareTo(_specifiedValue) <= 0;
        }
    }
}