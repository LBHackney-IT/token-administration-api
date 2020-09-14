using Microsoft.EntityFrameworkCore;

namespace TokenAdministrationApi.V1.Infrastructure
{

    public class TokenDatabaseContext : DbContext
    {
        public TokenDatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AuthTokens> Tokens { get; set; }
        public DbSet<ApiNameLookup> ApiNameLookups { get; set; }
        public DbSet<ApiEndpointNameLookup> ApiEndpointNameLookups { get; set; }
        public DbSet<ConsumerTypeLookup> ConsumerTypeLookups { get; set; }
    }
}
