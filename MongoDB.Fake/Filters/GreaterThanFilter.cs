namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;

    internal class GreaterThanFilter : IFilter
    {
        private readonly BsonValue _specifiedValue;

        public GreaterThanFilter(BsonValue specifiedValue)
        {
            _specifiedValue = specifiedValue;
        }

        public bool Filter(BsonValue value)
        {
            return value.CompareTo(_specifiedValue) > 0;
        }
    }
}
