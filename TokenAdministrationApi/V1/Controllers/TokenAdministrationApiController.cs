using System.Net.Mime;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Request;
using TokenAdministrationApi.V1.Domain.Exceptions;
using System.Reflection.Metadata.Ecma335;

namespace TokenAdministrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tokens")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TokenAdministrationApiController : BaseController
    {
        private readonly IGetAllTokensUseCase _getAllTokensUseCase;
        private readonly IPostTokenUseCase _postTokenUseCase;
        private readonly IUpdateTokenValidityUseCase _updateTokenValidity;
        private readonly IGetTokenOptionsUseCase _getTokenOptionsUseCase;
        private readonly IPostApiUseCase _postApiUsecase;

        private readonly IPostEndpointUseCase _postEndpointUsecase;


        public TokenAdministrationApiController(IGetAllTokensUseCase getAllTokensUseCase, IPostTokenUseCase postTokenUseCase,
            IUpdateTokenValidityUseCase updateTokenValidity, IGetTokenOptionsUseCase tokenOptionsUseCase, IPostApiUseCase postApiUsecase, IPostEndpointUseCase postEndpointUsecase)
        {
            _getAllTokensUseCase = getAllTokensUseCase;
            _postTokenUseCase = postTokenUseCase;
            _updateTokenValidity = updateTokenValidity;
            _getTokenOptionsUseCase = tokenOptionsUseCase;
            _postApiUsecase = postApiUsecase;
            _postEndpointUsecase = postEndpointUsecase;
        }

        /// <summary>
        /// Returns a list of all token records in the database with the optional filtering flag 'enabled'
        /// </summary>
        /// <response code="200">Returns a list of all token records</response>
        [ProducesResponseType(typeof(TokensListResponse), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListTokens([FromQuery] GetTokensRequest request)
        {
            return Ok(_getAllTokensUseCase.Execute(request));
        }

        /// <summary>
        /// Generates a token to be used for auth purposes for Hackney APIs
        /// </summary>
        /// <response code="201">Token successfully generated</response>
        /// <response code="400">One or more request parameters are invalid or missing</response>
        /// <response code="500">There was a problem generating a token.</response>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(GenerateTokenResponse), StatusCodes.Status201Created)]
        [HttpPost]
        public IActionResult GenerateToken([FromBody] TokenRequestObject tokenRequest)
        {
            try
            {
                var response = _postTokenUseCase.Execute(tokenRequest);
                return CreatedAtAction("GetToken", new { id = response.Id }, response);
            }
            catch (TokenNotInsertedException)
            {
                return StatusCode(500, "There was a problem inserting the token data into the database.");
            }
            catch (JwtTokenNotGeneratedException)
            {
                return StatusCode(500, "There was a problem generating a JWT token");
            }
            catch (LookupValueDoesNotExistException ex)
            {
                return StatusCode(400, $"One or more of the lookup ids provided is incorrect - {ex.Message}");
            }
        }

        [HttpPatch]
        [Route("{tokenId}")]
        public IActionResult UpdateToken(int tokenId, [FromBody] UpdateTokenRequest request)
        {
            try
            {
                _updateTokenValidity.Execute(tokenId, request);
            }
            catch (TokenRecordNotFoundException)
            {
                return NotFound("A token for this ID could not be found");
            }

            return NoContent();
        }

        [ProducesResponseType(typeof(TokenOptionsResponse), StatusCodes.Status200OK)]
        [HttpGet("options")]
        public IActionResult GetTokenOptions()
        {
            return Ok(_getTokenOptionsUseCase.Execute());
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiLookupOptionResponse), StatusCodes.Status201Created)]
        [HttpPost("apis")]
        public IActionResult PostApi([FromBody] CreateApiLookupRequest request)
        {
            var response = _postApiUsecase.Execute(request);
            return StatusCode(StatusCodes.Status201Created, response);

        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreateEndpointResponse), StatusCodes.Status201Created)]
        [HttpPost("apis/{apiLookupId}/endpoints")]
        public IActionResult PostEndpoint(int apiLookupId, [FromBody] CreateEndpointRequest request)
        {
            try
            {
                var response = _postEndpointUsecase.Execute(apiLookupId, request);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (LookupValueDoesNotExistException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicateEndpointException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
