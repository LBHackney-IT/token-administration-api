using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenAdministrationApi.V1.UseCase;
using TokenAdministrationApi.V1.Boundary.Request;
using System;

namespace TokenAdministrationApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/residents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class TokenAdministrationApiController : BaseController
    {
        private readonly GetAllTokensUseCase _getAllTokensUseCase;
        public TokenAdministrationApiController(GetAllTokensUseCase getAllTokensUseCase)
        {
            _getAllTokensUseCase = getAllTokensUseCase;
        }

        /// <summary>
        /// Returns a list of all token records in the database with the optional filtering flag 'enabled'
        /// </summary>
        /// <response code="200">Returns a list of all token records</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListTokens([FromQuery] GetTokensRequest request)
        {
            try
            {
                return Ok(_getAllTokensUseCase.Execute(request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
