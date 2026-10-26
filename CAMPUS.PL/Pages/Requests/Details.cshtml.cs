using System.ComponentModel.DataAnnotations;
using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.BLL.Models;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.PL.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAMPUS.PL.Pages.Requests;

public class DetailsModel : PageModel
{
    private readonly IRequestService _requests;
    private readonly ICommentService _comments;
    private readonly ICurrentUserService _currentUser;
    private readonly IStatusService _status;

    public DetailsModel(
        IRequestService requests,
        ICommentService comments,
        ICurrentUserService currentUser,
        IStatusService status)
    {
        _requests = requests;
        _comments = comments;
        _currentUser = currentUser;
        _status = status;
    }

    public ServiceRequest? Request { get; private set; }

    public RequestPermissions Permissions { get; private set; } = new();

    public List<RequestStatus> NextStatuses { get; private set; } = new();

    public List<Comment> Comments { get; private set; } = new();

    [BindProperty]
    [Display(Name = "Comment")]
    [Required(ErrorMessage = Messages.CommentRequired)]
    [StringLength(1000, ErrorMessage = Messages.CommentTooLong)]
    public string? NewComment { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var actor = await _currentUser.GetUserAsync();
        var loaded = await LoadAsync(id, actor);
        return loaded ?? Page();
    }

    public async Task<IActionResult> OnPostCommentAsync(int id)
    {
        var actor = await _currentUser.GetUserAsync();

        if (!ModelState.IsValid)
        {
            var loaded = await LoadAsync(id, actor);
            return loaded ?? Page();
        }

        var result = await _comments.AddAsync(id, NewComment, actor);
        switch (result.Kind)
        {
            case ResultKind.Forbidden:
                return RedirectToPage("/Account/AccessDenied");
            case ResultKind.NotFound:
                TempData["Error"] = result.Error;
                return RedirectToPage("/Index");
            case ResultKind.Invalid when result.Field is not null:
                ModelState.AddModelError(nameof(NewComment), result.Error!);
                var loaded = await LoadAsync(id, actor);
                return loaded ?? Page();
            case ResultKind.Invalid:
                TempData["Error"] = result.Error;
                return RedirectToPage(new { id });
        }

        TempData["Success"] = Messages.CommentAdded;
        return RedirectToPage(new { id });
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
        Permissions = _requests.GetPermissions(Request!, actor);
        NextStatuses = _status.GetNextStatuses(Request!.Status).ToList();

        var comments = await _comments.GetForRequestAsync(id, actor);
        if (comments.Succeeded && comments.Value is not null)
            Comments = comments.Value;

        return null;
    }
}
