using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Formatting;
using CAMPUS.PL.Auth;
using CAMPUS.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace CAMPUS.PL.Pages.Requests;

// Edit request: the reporter changes their request while it's open and not assigned (FR-04).
public class EditModel : RequestFormPageModel
{
    private readonly IRequestService _requests;
    private readonly ICurrentUserService _currentUser;

    public EditModel(IRequestService requests, ICurrentUserService currentUser, ILookupService lookups)
        : base(lookups)
    {
        _requests = requests;
        _currentUser = currentUser;
    }

    // Shows the form filled with the current request, if the reporter may still edit it.
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

        var request = result.Value!;
        if (!_requests.GetPermissions(request, actor).CanEdit)
        {
            TempData["Error"] = Messages.RequestLocked;
            return RedirectToPage("/Requests/Details", new { id });
        }

        ViewData["RefCode"] = id.ToRefCode();
        Input = RequestInputModel.FromRequest(request);
        await LoadListsAsync();
        return Page();
    }

    // Saves the changes through the service and handles the result.
    public async Task<IActionResult> OnPostAsync(int id)
    {
        ViewData["RefCode"] = id.ToRefCode();

        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return Page();
        }

        var actor = await _currentUser.GetUserAsync();
        var result = await _requests.UpdateAsync(id, Input.ToRequestInput(), actor);

        switch (result.Kind)
        {
            case ResultKind.Forbidden:
                return RedirectToPage("/Account/AccessDenied");

            case ResultKind.NotFound:
                TempData["Error"] = result.Error;
                return RedirectToPage("/Index");

            case ResultKind.Invalid when result.Field is not null:
                // A field error: show the form again with the error under that field.
                ModelState.AddModelError($"Input.{result.Field}", result.Error!);
                await LoadListsAsync();
                return Page();

            case ResultKind.Invalid:
                // A rule error, for example RequestLocked: back to the details with the message.
                TempData["Error"] = result.Error;
                return RedirectToPage("/Requests/Details", new { id });
        }

        TempData["Success"] = Messages.RequestUpdated;
        return RedirectToPage("/Requests/Details", new { id });
    }
}