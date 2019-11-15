namespace MongoDB.Fake.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson;

    internal class OrFilter : IFilter
    {
        private readonly IReadOnlyCollection<IFilter> _children;

        public OrFilter(IReadOnlyCollection<IFilter> children)
        {
            _children = children;
        }

        public bool Filter(BsonValue value)
        {
            return _children.Any(filter => filter.Filter(value));
        }
    }
}