using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.MongoDB.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer4.MongoDB.Models
{
    public class MongoDBUser : IMongoDBUser
    {
        public bool Enabled { get; set; }

        public string Password { get; set; }

        public string Provider { get; set; }

        public string ProviderId { get; set; }

        public string Salt { get; set; }

        [BsonId]
        public ObjectId Subject { get; set; }

        public string Username { get; set; }

        public IEnumerable<Claim> Claims { get; set; }

    }
}
