using System.ComponentModel.DataAnnotations;

namespace TokenAdministrationApi.V1.Boundary.Requests
{
    public class CreateApiLookupRequest
    {
        [Required]
        public string ApiName { get; set; }
        [Required]
        [MaxLength(16)]
        public string ApiGatewayId { get; set; }
    }
}
