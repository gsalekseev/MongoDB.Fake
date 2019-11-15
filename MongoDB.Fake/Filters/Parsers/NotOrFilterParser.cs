namespace MongoDB.Fake.Filters.Parsers
{
    using System.Collections.Generic;

    internal class NotOrFilterParser : AggregatorFilterParserBase
    {
        public NotOrFilterParser(IFilterParser rootFilterParser)
            : base(rootFilterParser)
        {
        }

        protected override IFilter CreateFilter(IReadOnlyCollection<IFilter> childrenFilters)
        {
            return new NotOrFilter(childrenFilters);
        }
    }
}
