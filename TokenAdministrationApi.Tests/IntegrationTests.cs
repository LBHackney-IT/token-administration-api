using System.Net.Http;
using TokenAdministrationApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NUnit.Framework;

namespace TokenAdministrationApi.Tests
{
    public class IntegrationTests<TStartup> where TStartup : class
    {
        protected HttpClient Client { get; private set; }
        protected TokenDatabaseContext DatabaseContext { get; private set; }

        private MockWebApplicationFactory<TStartup> _factory;
        private NpgsqlConnection _connection;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new NpgsqlConnection(ConnectionString.TestDatabase());
            _connection.Open();
            var npgsqlCommand = _connection.CreateCommand();
            npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
            npgsqlCommand.ExecuteNonQuery();
            _builder = new DbContextOptionsBuilder();
            _builder.UseNpgsql(_connection);
        }

        [SetUp]
        public void BaseSetup()
        {
            _factory = new MockWebApplicationFactory<TStartup>(_connection);
            Client = _factory.CreateClient();
            DatabaseContext = new TokenDatabaseContext(_builder.Options);
            DatabaseContext.Database.EnsureCreated();
            DatabaseContext.Tokens.RemoveRange(DatabaseContext.Tokens);
            DatabaseContext.ApiEndpointNameLookups.RemoveRange(DatabaseContext.ApiEndpointNameLookups);
            DatabaseContext.ApiNameLookups.RemoveRange(DatabaseContext.ApiNameLookups);
            DatabaseContext.ConsumerTypeLookups.RemoveRange(DatabaseContext.ConsumerTypeLookups);
            DatabaseContext.SaveChanges();
            _transaction = DatabaseContext.Database.BeginTransaction();
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
