namespace TokenAdministrationApi.V1.Boundary.Requests
{
    public class CreateApiLookupRequest
    {
        public string ApiName { get; set; }
        public string ApiGatewayId { get; set; }
    }
}
