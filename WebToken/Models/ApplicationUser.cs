using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebToken.Models
{
    public class ApplicationUser : IdentityUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public override string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("UserName")]
        public override string? UserName { get; set; } = string.Empty;
        [BsonElement("NormalizedUserName")]
        public override string? NormalizedUserName { get; set; } = string.Empty;

        [BsonElement("PasswordHash")]
        public override string? PasswordHash { get; set; } = string.Empty;
    }
}
