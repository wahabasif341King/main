using MongoDB.Driver;

public class MongoDBContext
{
    private readonly IMongoDatabase _Database; // IMongoDatabase ek Datatype hai jo _Database ko Hold Karey ga.

    public MongoDBContext (IConfiguration config) // IConfiguration "appsettigns.json" file ko read krta hai. Us se value uthata hai.
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb")); // MongoDBClient() Database se connection banata hai. 
        // config.GetConnectionString("MongoDb) Ye appsettings.json file se MongoDb ka Link Get krta hai or MongoDBClient ko deta hai.

        _Database = client.GetDatabase("TaskFlow");
    }

    public IMongoCollection<User> Users => _Database.GetCollection<User>("Users"); // IMongoCollection<User> us Collection ka Datatype hai jo hum Users se le rahy hain.
}