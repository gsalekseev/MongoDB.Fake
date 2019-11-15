namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal class LessThanFilterParser : IFilterParser
    {
        public IFilter Parse(BsonValue filter)
        {
            return new LessThanFilter(filter);
        }
    }
}