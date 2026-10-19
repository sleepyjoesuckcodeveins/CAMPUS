using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Formatting;
using CAMPUS.PL.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAMPUS.PL.Pages.Requests;

// Delete request: the reporter confirms before their request is removed (FR-04).
public class DeleteModel : PageModel
{
    private readonly IRequestService _requests;
    private readonly ICurrentUserService _currentUser;

    public DeleteModel(IRequestService requests, ICurrentUserService currentUser)
    {
        _requests = requests;
        _currentUser = currentUser;
    }

    // The request shown on the confirmation card.
    public ServiceRequest? Request { get; private set; }

    // Shows the confirmation, if the reporter may still delete the request.
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

        if (!_requests.GetPermissions(result.Value!, actor).CanDelete)
        {
            TempData["Error"] = Messages.RequestLocked;
            return RedirectToPage("/Requests/Details", new { id });
        }

        Request = result.Value;
        return Page();
    }

    // Deletes the request through the service and handles the result.
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var actor = await _currentUser.GetUserAsync();
        var result = await _requests.DeleteAsync(id, actor);

        switch (result.Kind)
        {
            case ResultKind.Forbidden:
                return RedirectToPage("/Account/AccessDenied");

            case ResultKind.NotFound:
                TempData["Error"] = result.Error;
                return RedirectToPage("/Index");

            case ResultKind.Invalid:
                // For example RequestLocked: it was assigned in the meantime.
                TempData["Error"] = result.Error;
                return RedirectToPage("/Requests/Details", new { id });
        }

        TempData["Success"] = string.Format(Messages.RequestDeleted, id.ToRefCode());
        return RedirectToPage("/Requests/Mine");
    }
}