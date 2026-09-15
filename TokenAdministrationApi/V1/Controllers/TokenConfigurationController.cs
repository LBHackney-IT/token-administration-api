using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Domain.Exceptions;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tokens")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TokenConfigurationController : BaseController
    {
        private readonly IGetTokenOptionsUseCase _getTokenOptionsUseCase;
        private readonly IPostApiUseCase _postApiUseCase;
        private readonly IPostEndpointUseCase _postEndpointUseCase;

        public TokenConfigurationController(IGetTokenOptionsUseCase getTokenOptionsUseCase,
            IPostApiUseCase postApiUseCase, IPostEndpointUseCase postEndpointUseCase)
        {
            _getTokenOptionsUseCase = getTokenOptionsUseCase;
            _postApiUseCase = postApiUseCase;
            _postEndpointUseCase = postEndpointUseCase;
        }

        [ProducesResponseType(typeof(TokenOptionsResponse), StatusCodes.Status200OK)]
        [HttpGet("options")]
        public async Task<IActionResult> GetTokenOptions()
        {
            var tokenOptions = await _getTokenOptionsUseCase.Execute();
            return Ok(tokenOptions.ToResponse());
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiLookupOptionResponse), StatusCodes.Status201Created)]
        [HttpPost("apis")]
        public IActionResult PostApi([FromBody] CreateApiLookupRequest request)
        {
            try
            {
                var api = _postApiUseCase.Execute(request.ToDomain());
                return StatusCode(StatusCodes.Status201Created, api.ToResponse());
            }
            catch (DuplicateApiException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreateEndpointResponse), StatusCodes.Status201Created)]
        [HttpPost("apis/{apiLookupId}/endpoints")]
        public IActionResult PostEndpoint(int apiLookupId, [FromBody] CreateEndpointRequest request)
        {
            try
            {
                var endpoint = _postEndpointUseCase.Execute(request.ToDomain(apiLookupId));
                return StatusCode(StatusCodes.Status201Created, endpoint.ToResponse());
            }
            catch (LookupValueDoesNotExistException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DuplicateEndpointException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
