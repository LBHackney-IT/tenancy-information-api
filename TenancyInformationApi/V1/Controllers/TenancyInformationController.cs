using System.Linq;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TenancyInformationApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/tenancies")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class TenancyInformationController : BaseController
    {
        private readonly IGetTenancyByIdUseCase _getByIdUseCase;
        public TenancyInformationController(IGetTenancyByIdUseCase getByIdUseCase)
        {
            _getByIdUseCase = getByIdUseCase;
        }

        /// <summary>
        /// Retrieve a single TenancyInformation object.
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">tag_ref is malformed or missing.</response>
        /// <response code="404">No tenancy was found for the provided tag_ref.</response>
        [ProducesResponseType(typeof(TenancyInformationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{tenancyReference}")]
        public IActionResult ViewRecord(string tenancyReference)
        {
            if (string.IsNullOrWhiteSpace(tenancyReference)) return BadRequest("No tag_ref provided.");

            tenancyReference = tenancyReference.Replace('-', '/');

            if (!ValidateTenancyReference(tenancyReference))
                return BadRequest("tag_ref is malformed or missing.");

            var result = _getByIdUseCase.Execute(tenancyReference);

            if (string.IsNullOrWhiteSpace(result.TenancyAgreementReference))
                return NotFound($"No tenancy was found for the provided tag_ref {tenancyReference}.");

            return Ok(result);
        }

        private static bool ValidateTenancyReference(string tenancyReference)
        {
            return !string.IsNullOrWhiteSpace(tenancyReference) &&
                   !tenancyReference.Any(char.IsLetter) &&
                   tenancyReference.Count(c => c == '/') is 1;
        }

        [HttpGet, Route("{badTenRef}/{caught}")]
        public IActionResult CatchSlashedReference() =>
            BadRequest("Replace '/' with '-' in provided tag_ref.");
    }
}
