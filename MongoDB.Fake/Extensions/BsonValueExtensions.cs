namespace MongoDB.Fake.Extensions
{
    using MongoDB.Bson;
    using System.Linq;

    public static class BsonValueExtensions
    {
        public static BsonValue Find(this BsonValue bsonValue, string name)
        {
            var fieldNames = name.Split('.').GetEnumerator();
            BsonValue currentBsonValue = bsonValue;
            while (fieldNames.MoveNext())
            {
                var keyToGet = (string)fieldNames.Current;
                if (currentBsonValue.IsBsonArray)
                {
                    var bsonArray = currentBsonValue.AsBsonArray;
                    if (int.TryParse(keyToGet, out int indexToGet))
                    {
                        currentBsonValue = bsonArray.ElementAt(indexToGet);
                    }
                    else if (keyToGet == "$")
                    {
                        currentBsonValue = bsonArray.ElementAt(0);
                    }
                    else 
                    {
                        var bsonValues = bsonArray.Select(x => x[keyToGet]);
                        if (bsonValues.Count() > 0)
                        {
                            currentBsonValue = new BsonArray(bsonValues).AsBsonValue;
                        }
                        else
                        {
                            return new BsonArray().AsBsonValue;
                        }
                    }
                }
                else if (currentBsonValue.IsBsonDocument)
                {
                    if (currentBsonValue.AsBsonDocument.Contains(keyToGet))
                    {
                        currentBsonValue = currentBsonValue[keyToGet];
                    }
                    else
                    {
                        return BsonNull.Value;
                    }
                }
                else
                {
                    return BsonNull.Value;
                }
            }
            return currentBsonValue;
        }

        public static BsonValue Update(this BsonValue bsonValue, string name, BsonValue value)
        {
            var fieldNames = name.Split('.');

            // Value to set on next level
            BsonValue valueForNextLevel = value;

            for (int i = 0; i < fieldNames.Length; i++)
            {
                var fieldName = fieldNames[fieldNames.Length - 1 - i];
                if (fieldName == "$")
                    fieldName = "0";

                var valueToTake = string.Join(".", fieldNames.Take(fieldNames.Length - 1 - i));
                var valueOfHigherLevel = string.IsNullOrEmpty(valueToTake) ? bsonValue : bsonValue.Find(valueToTake);
                if (valueOfHigherLevel.IsBsonDocument)
                {
                    valueForNextLevel = valueOfHigherLevel.AsBsonDocument.Set(fieldName, valueForNextLevel);
                }
                else if (valueOfHigherLevel.IsBsonArray)
                {
                    int fieldIndex = int.Parse(fieldName);
                    valueOfHigherLevel.AsBsonArray[fieldIndex] = valueForNextLevel;
                    valueForNextLevel = valueOfHigherLevel;
                }
            }

            return valueForNextLevel;
        }

        public static bool Exists(this BsonValue bsonValue, string name)
        {
            return bsonValue.Find(name) != BsonNull.Value;
        }
    }
}
