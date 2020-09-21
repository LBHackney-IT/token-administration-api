using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAdministrationApi.V1.Domain.Exceptions
{
    public class LookupValueDoesNotExistException : Exception
    {
        public LookupValueDoesNotExistException(string message)
              : base(message) { }
    }
}
