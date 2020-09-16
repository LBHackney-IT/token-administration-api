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
        public bool? Enabled { get; set; }
        public int Limit { get; set; } = 20;
        public int Cursor { get; set; } = 0;
    }
}
