namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;

    internal class EmptyFilter : IFilter
    {
        public bool Filter(BsonValue value)
        {
            return true;
        }
    }
}
