using System.ComponentModel.DataAnnotations;

namespace TokenAdministrationApi.V1.Boundary.Requests
{
    public class UpdateTokenRequest
    {
        [Required]
        public bool? Enabled { get; set; }
    }
}
