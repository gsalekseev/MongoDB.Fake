namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal class GreaterThanFilterParser : IFilterParser
    {
        public IFilter Parse(BsonValue filter)
        {
            return new GreaterThanFilter(filter);
        }
    }
}
