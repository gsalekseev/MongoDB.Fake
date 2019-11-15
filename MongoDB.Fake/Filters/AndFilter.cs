namespace MongoDB.Fake.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson;

    internal class AndFilter : IFilter
    {
        private readonly IReadOnlyCollection<IFilter> _children;

        public AndFilter(IReadOnlyCollection<IFilter> children)
        {
            _children = children;
        }

        public bool Filter(BsonValue value)
        {
            return _children.All(filter => filter.Filter(value));
        }
    }
}