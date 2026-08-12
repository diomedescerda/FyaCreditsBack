using FyaCredits.Application;
using FyaCredits.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FyaCredits.WebApi.Controllers;

[ApiController]
[Route("api/credits")]
[Authorize]
public sealed class CreditsController(CreditService creditService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreditResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreditResponse>> Register(
        RegisterCreditRequest request,
        CancellationToken cancellationToken)
    {
        var credit = await creditService.RegisterAsync(
            new RegisterCreditCommand(
                request.ClientName,
                request.ClientId,
                request.Amount,
                request.InterestRate,
                request.TermMonths),
            cancellationToken);

        return CreatedAtAction(nameof(Get), null, credit);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CreditResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CreditResponse>>> Get(
        [FromQuery] CreditQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (!new[] { "date", "amount" }.Contains(request.SortBy.ToLowerInvariant()))
            return Problem("sortBy must be date or amount.", statusCode: StatusCodes.Status400BadRequest);
        if (!new[] { "asc", "desc" }.Contains(request.SortDirection.ToLowerInvariant()))
            return Problem("sortDirection must be asc or desc.", statusCode: StatusCodes.Status400BadRequest);

        var result = await creditService.SearchAsync(
            new CreditQuery(
                request.Search,
                request.SortBy,
                request.SortDirection.ToLowerInvariant(),
                request.Page,
                request.PageSize),
            cancellationToken);

        return Ok(result);
    }
}
