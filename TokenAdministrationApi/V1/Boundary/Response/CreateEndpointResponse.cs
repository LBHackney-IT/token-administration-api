namespace TokenAdministrationApi.V1.Boundary.Response
{
    public class CreateEndpointResponse
    {
        public int Id { get; set; }
        public int ApiLookupId { get; set; }
        public string ApiName { get; set; }
        public string EndpointName { get; set; }
    }
}
