namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal class NotEqualFilterParser : IFilterParser
    {
        public IFilter Parse(BsonValue filter)
        {
            return new NotEqualFilter(filter);
        }
    }
}
