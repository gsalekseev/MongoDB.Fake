namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;
    using System.Linq;

    internal class EqualFilter : IFilter
    {
        private readonly BsonValue _specifiedValue;

        public EqualFilter(BsonValue specifiedValue)
        {
            _specifiedValue = specifiedValue;
        }

        public bool Filter(BsonValue value)
        {
            if (value.IsBsonArray)
            {
                return value.AsBsonArray.Any(x => x.Equals(_specifiedValue));
            }
            return value.Equals(_specifiedValue);
        }
    }
}
