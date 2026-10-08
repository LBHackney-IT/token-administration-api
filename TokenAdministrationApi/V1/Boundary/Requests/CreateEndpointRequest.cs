using System.ComponentModel.DataAnnotations;

namespace TokenAdministrationApi.V1.Boundary.Requests
{
    public class CreateEndpointRequest
    {
        [Required]
        public string EndpointName { get; set; }
    }
}
