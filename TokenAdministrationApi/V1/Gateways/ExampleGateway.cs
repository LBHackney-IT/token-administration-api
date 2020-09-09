using System.Collections.Generic;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.V1.Gateways
{
    //TODO: Rename to match the data source that is being accessed in the gateway eg. MosaicGateway
    public class ExampleGateway : IExampleGateway
    {
        private readonly TokenDatabaseContext _databaseContext;

        public ExampleGateway(TokenDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<AuthToken> GetAll()
        {
            return new List<AuthToken>();
        }
    }
}
