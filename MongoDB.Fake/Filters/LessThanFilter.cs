namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;

    internal class LessThanFilter : IFilter
    {
        private readonly BsonValue _specifiedValue;

        public LessThanFilter(BsonValue specifiedValue)
        {
            _specifiedValue = specifiedValue;
        }

        public bool Filter(BsonValue value)
        {
            return value.CompareTo(_specifiedValue) < 0;
        }
    }
}