using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Formatting;
using CAMPUS.PL.Auth;
using CAMPUS.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace CAMPUS.PL.Pages.Requests;

// New request: an employee reports an issue (FR-03).
public class CreateModel : RequestFormPageModel
{
    private readonly IRequestService _requests;
    private readonly ICurrentUserService _currentUser;

    public CreateModel(IRequestService requests, ICurrentUserService currentUser, ILookupService lookups)
        : base(lookups)
    {
        _requests = requests;
        _currentUser = currentUser;
    }

    // Shows the empty form with both dropdowns filled.
    public async Task OnGetAsync()
    {
        await LoadListsAsync();
    }

    // Sends the form to the service and handles the result.
    public async Task<IActionResult> OnPostAsync()
    {
        // 1. The quick check from the attributes on RequestInputModel
        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return Page();
        }

        // 2. The real check and the save, in the service
        var actor = await _currentUser.GetUserAsync();
        var result = await _requests.CreateAsync(Input.ToRequestInput(), actor);

        switch (result.Kind)
        {
            case ResultKind.Forbidden:
                return RedirectToPage("/Account/AccessDenied");

            case ResultKind.NotFound:
                TempData["Error"] = result.Error;
                return RedirectToPage("/Index");

            case ResultKind.Invalid:
                // A field error goes under that field; otherwise at the top of the form.
                var key = result.Field is null ? string.Empty : $"Input.{result.Field}";
                ModelState.AddModelError(key, result.Error!);
                await LoadListsAsync();
                return Page();
        }

        // 3. Success: back to My requests with the REQ code
        TempData["Success"] = string.Format(Messages.RequestCreated, result.Value.ToRefCode());
        return RedirectToPage("/Requests/Mine");
    }
}