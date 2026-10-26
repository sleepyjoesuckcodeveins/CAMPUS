using System.ComponentModel.DataAnnotations;
using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Formatting;
using CAMPUS.PL.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAMPUS.PL.Pages.Support;

public class UpdateModel : PageModel
{
    private readonly IRequestService _requests;
    private readonly IStatusService _status;
    private readonly ICurrentUserService _currentUser;

    public UpdateModel(IRequestService requests, IStatusService status, ICurrentUserService currentUser)
    {
        _requests = requests;
        _status = status;
        _currentUser = currentUser;
    }

    public ServiceRequest? Request { get; private set; }

    public List<RequestStatus> NextStatuses { get; private set; } = new();

    [BindProperty]
    [Display(Name = "New status")]
    [Required(ErrorMessage = Messages.StatusRequired)]
    public RequestStatus? NewStatus { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var actor = await _currentUser.GetUserAsync();
        var loaded = await LoadAsync(id, actor);
        return loaded ?? Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var actor = await _currentUser.GetUserAsync();

        if (!ModelState.IsValid)
        {
            var loaded = await LoadAsync(id, actor);
            return loaded ?? Page();
        }

        var result = await _status.ChangeStatusAsync(id, NewStatus!.Value, actor);
        switch (result.Kind)
        {
            case ResultKind.Forbidden:
                return RedirectToPage("/Account/AccessDenied");
            case ResultKind.NotFound:
                TempData["Error"] = result.Error;
                return RedirectToPage("/Index");
            case ResultKind.Invalid when result.Field is not null:
                ModelState.AddModelError(nameof(NewStatus), result.Error!);
                var loaded = await LoadAsync(id, actor);
                return loaded ?? Page();
            case ResultKind.Invalid:
                TempData["Error"] = result.Error;
                return RedirectToPage("/Requests/Details", new { id });
        }

        TempData["Success"] = string.Format(Messages.StatusChanged, id.ToRefCode(), NewStatus!.Value.ToLabel());
        return RedirectToPage("/Requests/Details", new { id });
    }

    private async Task<IActionResult?> LoadAsync(int id, User actor)
    {
        var result = await _requests.GetForViewAsync(id, actor);
        if (result.Kind == ResultKind.Forbidden)
            return RedirectToPage("/Account/AccessDenied");

        if (result.Kind == ResultKind.NotFound)
        {
            TempData["Error"] = result.Error;
            return RedirectToPage("/Index");
        }

        Request = result.Value;
        if (!_requests.GetPermissions(Request!, actor).CanChangeStatus)
            return RedirectToPage("/Account/AccessDenied");

        NextStatuses = _status.GetNextStatuses(Request!.Status).ToList();
        if (NextStatuses.Count == 0)
        {
            TempData["Error"] = Messages.RequestAlreadyResolved;
            return RedirectToPage("/Requests/Details", new { id });
        }

        if (!NewStatus.HasValue || !NextStatuses.Contains(NewStatus.Value))
            NewStatus = NextStatuses[0];

        return null;
    }
}
