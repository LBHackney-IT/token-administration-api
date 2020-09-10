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
    [Route("api/v1/tokens")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TokenAdministrationApiController : BaseController
    {
        private readonly IGetAllTokensUseCase _getAllTokensUseCase;
        public TokenAdministrationApiController(IGetAllTokensUseCase getAllTokensUseCase)
        {
            _getAllTokensUseCase = getAllTokensUseCase;
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
    }
}
