using TokenAdministrationApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;

namespace TokenAdministrationApi.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        protected TokenDatabaseContext DatabaseContext { get; private set; }
        private IDbContextTransaction _transaction;


        [SetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(ConnectionString.TestDatabase());
            DatabaseContext = new TokenDatabaseContext(builder.Options);
            //clear existing data
            DatabaseContext.Database.EnsureCreated();
            DatabaseContext.Tokens.RemoveRange(DatabaseContext.Tokens);
            DatabaseContext.ApiEndpointNameLookups.RemoveRange(DatabaseContext.ApiEndpointNameLookups);
            DatabaseContext.ApiNameLookups.RemoveRange(DatabaseContext.ApiNameLookups);
            DatabaseContext.ConsumerTypeLookups.RemoveRange(DatabaseContext.ConsumerTypeLookups);
            DatabaseContext.SaveChanges();
            _transaction = DatabaseContext.Database.BeginTransaction();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
