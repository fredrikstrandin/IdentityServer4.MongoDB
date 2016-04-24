using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Core.MongoDB.Serializer;
using IdentityServer4.Core.Validation;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace IdSvrHost.MongoDB
{
    public class Startup
    {
        private readonly IApplicationEnvironment _environment;

        public Startup(IApplicationEnvironment environment)
        {
            _environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var _client = new MongoClient();

            BsonSerializer.RegisterSerializationProvider(new ClaimProvider());

            services.AddInstance<IMongoDatabase>(_client.GetDatabase("IdentityServer"));

            services.AddMongoDBTransientStores();

            var builder = services.AddIdentityServer(options =>
            {
                options.SigningCertificate = new X509Certificate2(Path.Combine(_environment.ApplicationBasePath, "idsrv4test.pfx"), "idsrv3test");
            });

            builder.AddMongoDBUsers();
            builder.AddMongoDBScopes();
            builder.AddMongoDBClients();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Verbose);
            loggerFactory.AddDebug(LogLevel.Verbose);

            var sourceEventSwitch = new SourceSwitch("LoggingSample");
            sourceEventSwitch.Level = SourceLevels.Critical;

            loggerFactory.AddTraceSource(sourceEventSwitch,
                new EventLogTraceListener("Application"));

            var sourceFileSwitch = new SourceSwitch("LoggingSample");
            sourceFileSwitch.Level = SourceLevels.All;

            loggerFactory.AddTraceSource(sourceFileSwitch,
                new TextWriterTraceListener(@"\log\IdSvrHost.log")
            );

            app.UseDeveloperExceptionPage();
            app.UseIISPlatformHandler();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
