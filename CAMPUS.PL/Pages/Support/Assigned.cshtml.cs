using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.PL.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAMPUS.PL.Pages.Support;

// Assigned to me: a support member sees the requests of their own team.
public class AssignedModel : PageModel
{
    private readonly IOverviewService _overview;
    private readonly ICurrentUserService _currentUser;

    public AssignedModel(IOverviewService overview, ICurrentUserService currentUser)
    {
        _overview = overview;
        _currentUser = currentUser;
    }

    // The status filter from the URL (?status=InProgress).
    [BindProperty(SupportsGet = true, Name = "status")]
    public RequestStatus? Status { get; set; }

    public bool HasTeam { get; private set; }

    public string TeamName { get; private set; } = string.Empty;

    public List<ServiceRequest> Requests { get; private set; } = new();

    // Loads the team's requests, or shows the no-team message.
    public async Task<IActionResult> OnGetAsync()
    {
        var actor = await _currentUser.GetUserAsync();
        var result = await _overview.GetAssignedAsync(Status, actor);

        if (result.Kind == ResultKind.Forbidden)
            return RedirectToPage("/Account/AccessDenied");

        HasTeam = actor.TeamId.HasValue;
        TeamName = actor.Team?.Name ?? string.Empty;
        Requests = result.Value!;
        return Page();
    }
}