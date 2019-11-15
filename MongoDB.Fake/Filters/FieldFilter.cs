using MongoDB.Bson;
using System.Linq;

namespace MongoDB.Fake.Filters
{
    internal class FieldFilter : IFilter
    {
        private readonly string _fieldName;
        private readonly IFilter _child;

        public FieldFilter(string fieldName, IFilter child)
        {
            _fieldName = fieldName;
            _child = child;
        }

        public bool Filter(BsonValue value)
        {
            if (TryGetFieldValue(value, out BsonValue fieldValue))
            {
                return _child.Filter(fieldValue);
            }
            return false;
        }

        private bool TryGetFieldValue(BsonValue inputValue, out BsonValue fieldValue)
        {
            fieldValue = null;
            if (!inputValue.IsBsonDocument)
            {
                return false;
            }

            var document = inputValue.AsBsonDocument;

            if (!Contains(document, out fieldValue))
            {
                return false;
            }
            return true;
        }

        private bool Contains(BsonDocument document, out BsonValue fieldValue)
        {
            fieldValue = null;
            if (_fieldName.Contains("."))
            {
                var fieldNames = _fieldName.Split('.').GetEnumerator();
                BsonValue currentBsonValue = document;
                while (fieldNames.MoveNext())
                {
                    if (currentBsonValue.IsBsonArray)
                    {
                        var bsonArray = currentBsonValue.AsBsonArray;
                        var bsonValues = bsonArray.Select(x => x[(string)fieldNames.Current]);
                        if (bsonValues.Count() > 0)
                        {
                            currentBsonValue = new BsonArray(bsonValues).AsBsonValue;
                        }
                    }
                    else if (currentBsonValue.IsBsonDocument)
                    {
                        if (currentBsonValue.AsBsonDocument.Contains((string)fieldNames.Current))
                        {
                            currentBsonValue = currentBsonValue[(string)fieldNames.Current];
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                fieldValue = currentBsonValue;
            }
            else
            {
                if (!document.Contains(_fieldName))
                {
                    return false;
                }
                fieldValue = document[_fieldName];
            }
            return true;
        }
    }
}