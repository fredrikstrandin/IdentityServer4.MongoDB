﻿// Copyright (c) Fredrik Strandin. All rights reserved.
// This is based on InMemoryCorsPolicyService from IdentityServer
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using IdentityServer4.Core.MongoDB.Models;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// CORS policy service that configures the allowed origins from a list of clients' redirect URLs.
    /// </summary>
    public class MongoDBCorsPolicyService : ICorsPolicyService
    {
        private readonly string _collectionClients = "Clients";
        private readonly ILogger<MongoDBCorsPolicyService> _logger;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBCorsPolicyService"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        public MongoDBCorsPolicyService(ILogger<MongoDBCorsPolicyService> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var urls = await _database.GetCollection<Client>(_collectionClients)
                .Aggregate()
                .Unwind(x => x.AllowedCorsOrigins, new AggregateUnwindOptions<Client>() { PreserveNullAndEmptyArrays = true })
                       .Project<CorsOrigin>(Builders<Client>.Projection
                                                .Include("AllowedCorsOrigins")
                                                .Exclude("_id"))
                .ToListAsync();

            var origins = urls.Select(x => x.AllowedCorsOrigins.GetOrigin())
                .Where(x => x != null)
                .Distinct();

            var result = origins.Contains(origin, StringComparer.OrdinalIgnoreCase);

            if (result)
            {
                _logger.LogInformation("Client list checked and origin: {0} is allowed", origin);
            } else
            {
                _logger.LogInformation("Client list checked and origin: {0} is not allowed", origin);
            }

            return await Task.FromResult(result);

            //return await Task.FromResult(true);
        }
    }
}
