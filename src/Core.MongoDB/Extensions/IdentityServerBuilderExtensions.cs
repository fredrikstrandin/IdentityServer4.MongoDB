using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Core.Services;
using IdentityServer4.Core.Services.MongoDB;
using IdentityServer4.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.MongoDB.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoDBUsers(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IProfileService, MongoDBProfileService>();
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, MongoDBResourceOwnerPasswordValidator>();

            return builder;
        }

        public static IIdentityServerBuilder AddMongoDBClients(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, MongoDBClientStore>();
            builder.Services.AddTransient<ICorsPolicyService, MongoDBCorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddMongoDBScopes(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IScopeStore, MongoDBScopeStore>();

            return builder;
        }

        public static IServiceCollection AddMongoDBTransientStores(this IServiceCollection services)
        {
            services.TryAddSingleton<IAuthorizationCodeStore, MongoDBAuthorizationCodeStore>();
            services.TryAddSingleton<IRefreshTokenStore, MongoDBRefreshTokenStore>();
            services.TryAddSingleton<ITokenHandleStore, MongoDBTokenHandleStore>();
            services.TryAddSingleton<IConsentStore, MongoDBConsentStore>();

            return services;
        }
    }
}
