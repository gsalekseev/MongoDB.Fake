namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal class LessThanOrEqualFilterParser : IFilterParser
    {
        public IFilter Parse(BsonValue filter)
        {
            return new LessThanOrEqualFilter(filter);
        }
    }
}