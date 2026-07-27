public class User
{
    [MongoDB.Bson.Serialization.Attributes.BsonId]
    [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Username { get; set; }

    public string? Email_Address { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
}