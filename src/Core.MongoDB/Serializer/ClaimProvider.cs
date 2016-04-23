using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace IdentityServer4.Core.MongoDB.Serializer
{
    public class ClaimProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == typeof(Claim))
            {
                return new ClaimSerializer();
            }

            return null;
        }
    }
}
