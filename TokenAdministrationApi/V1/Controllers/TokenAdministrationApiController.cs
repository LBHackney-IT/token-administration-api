using System.Net.Mime;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Request;
using TokenAdministrationApi.V1.Domain.Exceptions;

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

        public TokenAdministrationApiController(IGetAllTokensUseCase getAllTokensUseCase, IPostTokenUseCase postTokenUseCase,
            IUpdateTokenValidityUseCase updateTokenValidity)
        {
            _getAllTokensUseCase = getAllTokensUseCase;
            _postTokenUseCase = postTokenUseCase;
            _updateTokenValidity = updateTokenValidity;
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
            catch (DbUpdateException)
            {
                return StatusCode(400, "One or more of the lookup ids provided is incorrect");
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
    }
}
