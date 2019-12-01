namespace MongoDB.Fake.Filters
{
    using MongoDB.Bson;
    using MongoDB.Fake.Extensions;
    using System.Linq;

    internal class ElemMatchFilter : IFilter
    {
        private readonly BsonValue _specifiedValue;

        public ElemMatchFilter(BsonValue specifiedValue)
        {
            _specifiedValue = specifiedValue;
        }

        public bool Filter(BsonValue value)
        {
            if (value.IsBsonArray)
            {
                if (_specifiedValue.IsBsonDocument)
                {
                    var tempValue = _specifiedValue.AsBsonDocument;
                    for (int i = 0; i < tempValue.ElementCount; i++)
                    {
                        var documentValues = value.AsBsonValue.Find(tempValue.GetElement(i).Name);
                        var exceptedValue = tempValue.GetElement(i).Value;
                        return documentValues.AsBsonArray.Any(x => x.Equals(exceptedValue));
                    }
                }

                return value.AsBsonArray.Any(x => x.Equals(_specifiedValue));
            }
            return value.Equals(_specifiedValue);
        }
    }
}
