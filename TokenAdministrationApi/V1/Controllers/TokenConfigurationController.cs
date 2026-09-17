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
    [Route("api/v1/token-configuration")]
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

        /// <summary>
        /// Returns the consumer types, APIs and endpoints available when creating a token.
        /// </summary>
        /// <response code="200">Returns the available token configuration options.</response>
        [ProducesResponseType(typeof(TokenOptionsResponse), StatusCodes.Status200OK)]
        [HttpGet("options")]
        public async Task<IActionResult> GetTokenOptions()
        {
            var tokenOptions = await _getTokenOptionsUseCase.Execute();
            return Ok(tokenOptions.ToResponse());
        }

        /// <summary>
        /// Creates an API that can be selected when creating a token.
        /// </summary>
        /// <response code="201">The API was created successfully.</response>
        /// <response code="400">One or more request values are invalid or missing.</response>
        /// <response code="409">An API with the same name or API Gateway ID already exists.</response>
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

        /// <summary>
        /// Creates an endpoint for an existing API.
        /// </summary>
        /// <response code="201">The endpoint was created successfully.</response>
        /// <response code="400">One or more request values are invalid or missing.</response>
        /// <response code="404">The selected API could not be found.</response>
        /// <response code="409">The endpoint already exists for the selected API.</response>
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
