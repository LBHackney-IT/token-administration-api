using System.ComponentModel.DataAnnotations;

namespace TokenAdministrationApi.V1.Boundary.Requests
{
    public class CreateEndpointRequest
    {
        [Required]
        [MaxLength(255)]
        public string EndpointName { get; set; }
    }
}
