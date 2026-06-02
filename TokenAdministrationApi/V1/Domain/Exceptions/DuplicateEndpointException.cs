using System;

namespace TokenAdministrationApi.V1.Domain.Exceptions
{
    public class DuplicateEndpointException : Exception
    {
        public DuplicateEndpointException(string message)
            : base(message) { }
    }
}
