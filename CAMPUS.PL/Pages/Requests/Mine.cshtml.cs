using CAMPUS.BLL.Interfaces;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.PL.Auth;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAMPUS.PL.Pages.Requests;

// My requests: the employee's own requests, newest first (FR-04).
public class MineModel : PageModel
{
    private readonly IRequestService _requests;
    private readonly ICurrentUserService _currentUser;

    // The logged-in user, kept for CanEdit.
    private User? _actor;

    public MineModel(IRequestService requests, ICurrentUserService currentUser)
    {
        _requests = requests;
        _currentUser = currentUser;
    }

    public List<ServiceRequest> Requests { get; private set; } = new();

    // Loads the logged-in employee's requests.
    public async Task OnGetAsync()
    {
        _actor = await _currentUser.GetUserAsync();
        Requests = await _requests.GetMineAsync(_actor);
    }

    // True when the Edit and Delete buttons may be shown for this request.
    public bool CanEdit(ServiceRequest request)
    {
        return _actor is not null && _requests.GetPermissions(request, _actor).CanEdit;
    }
}