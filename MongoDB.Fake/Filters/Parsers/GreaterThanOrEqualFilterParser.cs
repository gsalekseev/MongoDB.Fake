namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal class GreaterThanOrEqualFilterParser : IFilterParser
    {
        public IFilter Parse(BsonValue filter)
        {
            return new GreaterThanOrEqualFilter(filter);
        }
    }
}