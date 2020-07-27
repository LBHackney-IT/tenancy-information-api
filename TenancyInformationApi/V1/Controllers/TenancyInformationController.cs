using System;
using System.Linq;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TenancyInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tenancies")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TenancyInformationController : BaseController
    {
        private readonly IGetTenancyByIdUseCase _getTenancyByIdUseCase;

        public TenancyInformationController(
            IGetTenancyByIdUseCase getTenancyByIdUseCase
        )
        {
            _getTenancyByIdUseCase = getTenancyByIdUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-base-api/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListContacts()
        {
            throw new NotImplementedException();
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
            tenancyReference = tenancyReference.Replace('-', '/');

            if (!ValidateTenancyReference(tenancyReference))
                return BadRequest("tag_ref is malformed or missing.");

            var result = _getTenancyByIdUseCase.Execute(tenancyReference);

            if (result.TenancyReference is null)
                return NotFound($"No tenancy was found for the provided tag_ref {tenancyReference}.");

            return Ok(result);
        }

        private static bool ValidateTenancyReference(string tenancyReference)
        {
            return !string.IsNullOrWhiteSpace(tenancyReference) &&
                   !tenancyReference.Any(char.IsLetter) &&
                   tenancyReference.Count(c => c == '/') is 1;
        }

        // TODO: catch tag_refs entered with a slash instead of hyphen
        // Two ideas:
        [HttpGet, Route("{badTenRef}/{caught}")]
        public IActionResult CatchSlashedReference(string badTenRef, string caught) =>
            // BadRequest("Replace '/' with '-' in provided tag_ref.");
            ViewRecord(badTenRef + '-' + caught);
    }
}
