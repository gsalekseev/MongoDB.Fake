namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal class ElemMatchFilterParser : IFilterParser
    {
        public IFilter Parse(BsonValue filter)
        {
            return new ElemMatchFilter(filter);
        }
    }
}
