using System;
using System.Linq;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TenancyInformationApi.V1.Boundary;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.UseCase;

namespace TenancyInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tenancies")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TenancyInformationController : BaseController
    {
        private readonly IGetTenancyByIdUseCase _getByIdUseCase;
        private readonly IListTenancies _listTenancies;

        public TenancyInformationController(IGetTenancyByIdUseCase getByIdUseCase, IListTenancies listTenancies)
        {
            _getByIdUseCase = getByIdUseCase;
            _listTenancies = listTenancies;
        }

        /// <summary>
        /// Lists and queries tenancies.
        /// </summary>
        /// <response code="200">Success!</response>
        /// <response code="400">Bad Request. One of the query parameters provided was invalid.</response>
        [ProducesResponseType(typeof(ListTenanciesResponse), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListTenancies([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                return Ok(_listTenancies.Execute(queryParameters.Limit, queryParameters.Cursor, queryParameters.Address,
                    queryParameters.Postcode, queryParameters.LeaseholdsOnly, queryParameters.FreeholdsOnly));

            }
            catch (InvalidQueryParameterException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Retrieve a single TenancyInformation object.
        /// </summary>
        /// <response code="200">Success!</response>
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

            if (result == null)
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
