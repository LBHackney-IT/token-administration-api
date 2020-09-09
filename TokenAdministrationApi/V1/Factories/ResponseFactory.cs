using System.Collections.Generic;
using System.Linq;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Factories
{
    public static class ResponseFactory
    {
        //TODO: Map the fields in the domain object(s) to fields in the response object(s).
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-TokenAdministrationApi/wiki/Factory-object-mappings
        public static ResponseObject ToResponse(this AuthToken domain)
        {
            return new ResponseObject();
        }

        public static List<ResponseObject> ToResponse(this IEnumerable<AuthToken> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
