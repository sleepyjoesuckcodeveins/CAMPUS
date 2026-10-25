using System.ComponentModel.DataAnnotations;
using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Formatting;
using CAMPUS.PL.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CAMPUS.PL.Pages.Manager;

// Assign request: the manager chooses the support team for a request (FR-06).
public class AssignModel : PageModel
{
    private readonly IRequestService _requests;
    private readonly IAssignmentService _assignments;
    private readonly ICurrentUserService _currentUser;
    private readonly ILookupService _lookups;

    public AssignModel(
        IRequestService requests,
        IAssignmentService assignments,
        ICurrentUserService currentUser,
        ILookupService lookups)
    {
        _requests = requests;
        _assignments = assignments;
        _currentUser = currentUser;
        _lookups = lookups;
    }

    // The request shown in the left card.
    public ServiceRequest? Request { get; private set; }

    // The chosen team. Nullable, so an empty dropdown gives our own message.
    [BindProperty]
    [Display(Name = "Team")]
    [Required(ErrorMessage = Messages.TeamRequired)]
    public int? TeamId { get; set; }

    public List<SelectListItem> Teams { get; private set; } = new();

    // Shows the request and the team dropdown, with the current team selected.
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var actor = await _currentUser.GetUserAsync();
        var result = await _requests.GetForViewAsync(id, actor);

        if (result.Kind == ResultKind.Forbidden)
            return RedirectToPage("/Account/AccessDenied");

        if (result.Kind == ResultKind.NotFound)
        {
            TempData["Error"] = result.Error;
            return RedirectToPage("/Index");
        }

        Request = result.Value;
        TeamId = Request!.AssignedTeamId;
        await LoadTeamsAsync();
        return Page();
    }

    // Assigns the chosen team through the service and handles the result.
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var actor = await _currentUser.GetUserAsync();

        if (ModelState.IsValid)
        {
            var result = await _assignments.AssignAsync(id, TeamId!.Value, actor);

            switch (result.Kind)
            {
                case ResultKind.Success:
                    await LoadTeamsAsync();
                    var teamName = Teams.FirstOrDefault(t => t.Value == TeamId.ToString())?.Text;
                    TempData["Success"] = string.Format(Messages.RequestAssigned, id.ToRefCode(), teamName);
                    return RedirectToPage("/Manager/AllRequests");

                case ResultKind.Forbidden:
                    return RedirectToPage("/Account/AccessDenied");

                case ResultKind.NotFound:
                    TempData["Error"] = result.Error;
                    return RedirectToPage("/Index");

                case ResultKind.Invalid when result.Field is null:
                    // For example: the request was resolved in the meantime.
                    TempData["Error"] = result.Error;
                    return RedirectToPage("/Requests/Details", new { id });

                case ResultKind.Invalid:
                    ModelState.AddModelError(result.Field!, result.Error!);
                    break;
            }
        }

        // Something is wrong with the choice: show the page again with the error.
        var view = await _requests.GetForViewAsync(id, actor);
        if (view.Value is null)
            return RedirectToPage("/Index");

        Request = view.Value;
        await LoadTeamsAsync();
        return Page();
    }

    // Fills the team dropdown.
    private async Task LoadTeamsAsync()
    {
        var teams = await _lookups.GetTeamsAsync();
        Teams = teams
            .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
            .ToList();
    }
}