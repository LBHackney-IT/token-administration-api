using System;

namespace TokenAdministrationApi.V1.Domain
{
    public class GenerateJwtRequest
    {
        public int Id { get; set; }
        public int ConsumerType { get; set; }
        public string ConsumerName { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
