using System.Linq;
using TokenAdministrationApi.Tests.V1.Helper;
using TokenAdministrationApi.V1.Infrastructure;
using NUnit.Framework;
using FluentAssertions;

namespace TokenAdministrationApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class DatabaseContextTest : DatabaseTests
    {
        [Test]
        public void CanGetADatabaseEntity()
        {
            //remove any record that might be left in the table
            DatabaseContext.RemoveRange(DatabaseContext.Tokens);
            var databaseEntity = AuthTokenDatabaseEntityHelper.CreateDatabaseEntity();

            DatabaseContext.Add(databaseEntity);
            DatabaseContext.SaveChanges();

            var result = DatabaseContext.Tokens.ToList().FirstOrDefault();

            result.Should().Be(databaseEntity);
        }
    }
}
