namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;

    internal class NotFilter : IFilter
    {
        private readonly IFilter _child;

        public NotFilter(IFilter child)
        {
            _child = child;
        }

        public bool Filter(BsonValue value)
        {
            return !_child.Filter(value);
        }
    }
}
