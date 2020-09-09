using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAdministrationApi.V1.Boundary.Request
{
    public class GetTokensRequest
    {
        /// <example>
        /// true
        /// </example>
        public bool Enabled { get; set; }
    }
}
