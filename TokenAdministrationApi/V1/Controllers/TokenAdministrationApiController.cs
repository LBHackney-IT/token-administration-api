using System.Net.Mime;
using ApiAuthTokenGenerator.V1.Boundary.Response;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tokens")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TokenAdministrationApiController : BaseController
    {
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly IPostTokenUseCase _postTokenUseCase;

        public TokenAdministrationApiController(IGetAllUseCase getAllUseCase, IPostTokenUseCase postTokenUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _postTokenUseCase = postTokenUseCase;
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListContacts()
        {
            return Ok(_getAllUseCase.Execute());
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
        }
    }
}
