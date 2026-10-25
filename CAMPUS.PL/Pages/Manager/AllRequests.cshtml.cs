using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.PL.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CAMPUS.PL.Pages.Manager;

// All requests: managers and administrators see every request, with filters (FR-05).
public class AllRequestsModel : PageModel
{
    private readonly IOverviewService _overview;
    private readonly ICurrentUserService _currentUser;
    private readonly ILookupService _lookups;

    public AllRequestsModel(IOverviewService overview, ICurrentUserService currentUser, ILookupService lookups)
    {
        _overview = overview;
        _currentUser = currentUser;
        _lookups = lookups;
    }

    // The filters come from the URL (?status=Open&campusId=1), so a filtered list can be bookmarked.
    [BindProperty(SupportsGet = true, Name = "status")]
    public RequestStatus? Status { get; set; }

    [BindProperty(SupportsGet = true, Name = "campusId")]
    public int? CampusId { get; set; }

    public List<SelectListItem> Campuses { get; private set; } = new();

    public List<ServiceRequest> Requests { get; private set; } = new();

    // Loads the filtered list and the campuses for the filter dropdown.
    public async Task<IActionResult> OnGetAsync()
    {
        var actor = await _currentUser.GetUserAsync();
        var result = await _overview.GetAllAsync(Status, CampusId, actor);

        if (result.Kind == ResultKind.Forbidden)
            return RedirectToPage("/Account/AccessDenied");

        Requests = result.Value!;

        var campuses = await _lookups.GetCampusesAsync();
        Campuses = campuses
            .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == CampusId))
            .ToList();

        return Page();
    }
}