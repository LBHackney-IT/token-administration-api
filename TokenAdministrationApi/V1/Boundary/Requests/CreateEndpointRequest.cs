namespace TokenAdministrationApi.V1.Boundary.Requests
{
    public class CreateEndpointRequest
    {
        public int ApiLookupId { get; set; }
        public string EndpointName { get; set; }
    }
}