namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;

    internal interface IFilter
    {
        bool Filter(BsonValue value);
    }
}