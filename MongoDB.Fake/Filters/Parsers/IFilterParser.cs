namespace MongoDB.Fake.Filters.Parsers
{
    using MongoDB.Bson;

    internal interface IFilterParser
    {
       IFilter Parse(BsonValue filter);
    }
}
