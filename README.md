# Start using

var mongoClient = new FakeMongoClient();  
var database = mongoClient.GetDatabase("test_database");  
var collection = database.GetCollection<TestClass>("test_collection"); 

collection.InsertOne(new TestClass());  
