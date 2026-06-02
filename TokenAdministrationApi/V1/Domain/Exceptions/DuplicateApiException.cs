using System;

namespace TokenAdministrationApi.V1.Domain.Exceptions
{
    public class DuplicateApiException : Exception
    {
        public DuplicateApiException(string message)
            : base(message) { }
    }
}
