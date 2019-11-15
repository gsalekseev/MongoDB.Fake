namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal class EqualFilterParser : IFilterParser
    {
        public IFilter Parse(BsonValue filter)
        {
            return new EqualFilter(filter);
        }
    }
}
