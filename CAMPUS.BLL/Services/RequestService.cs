using CAMPUS.BLL.Common;
using CAMPUS.BLL.Interfaces;
using CAMPUS.BLL.Models;
using CAMPUS.BLL.Notifications;
using CAMPUS.BLL.Rules;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Repositories;
using System.Security.AccessControl;

namespace CAMPUS.BLL.Services;

// Reporting and my requests: create, view, edit and delete a request (FR-03, FR-04).
public class RequestService : IRequestService
{
    private readonly IServiceRequestRepository _requests;
    private readonly ILookupRepository _lookups;
    private readonly INotificationService _notifications;
    private readonly TimeProvider _time;

    public RequestService(
        IServiceRequestRepository requests,
        ILookupRepository lookups,
        INotificationService notifications,
        TimeProvider time)
    {
        _requests = requests;
        _lookups = lookups;
        _notifications = notifications;
        _time = time;
    }

    // Creates a new open request for the employee, then e-mails the facilities managers.
    public async Task<ServiceResult<int>> CreateAsync(RequestInput input, User actor)
    {
        if (!AccessRules.CanCreate(actor))
            return ServiceResult<int>.Forbidden();

        var check = RequestRules.Validate(input);
        if (!check.Succeeded)
            return ServiceResult<int>.Invalid(check.Error!, check.Field);

        var exists = await CheckCategoryAndCampusAsync(input);
        if (!exists.Succeeded)
            return ServiceResult<int>.Invalid(exists.Error!, exists.Field);

        var request = new ServiceRequest
        {
            Title = input.Title.Trim(),
            Description = input.Description.Trim(),
            CategoryId = input.CategoryId,
            CampusId = input.CampusId,
            Location = input.Location.Trim(),
            Priority = input.Priority,
            Status = RequestStatus.Open,
            ReporterId = actor.Id,
            CreatedAt = Now()
        };
        await _requests.AddAsync(request);

        // Load it again, so the e-mail has the reporter, category and campus names.
        var saved = await _requests.GetByIdAsync(request.Id);
        if (saved is not null)
            await _notifications.RequestSubmittedAsync(saved);

        return ServiceResult<int>.Success(request.Id);
    }

    // The actor's own requests, newest first.
    public Task<List<ServiceRequest>> GetMineAsync(User actor)
    {
        return _requests.GetByReporterAsync(actor.Id);
    }

    // One request, if the actor may see it.
    public async Task<ServiceResult<ServiceRequest>> GetForViewAsync(int id, User actor)
    {
        var request = await _requests.GetByIdAsync(id);
        if (request is null)
            return ServiceResult<ServiceRequest>.NotFound(Messages.RequestNotFound);

        if (!AccessRules.CanView(request, actor))
            return ServiceResult<ServiceRequest>.Forbidden();

        return ServiceResult<ServiceRequest>.Success(request);
    }

    // Which buttons the actor may see for this request.
    public RequestPermissions GetPermissions(ServiceRequest request, User actor)
    {
        var isReporter = request.ReporterId == actor.Id;
        var isOpenForChanges = request.Status != RequestStatus.Resolved;
        var editable = isReporter && RequestRules.IsEditable(request);

        return new RequestPermissions
        {
            CanEdit = editable,
            CanDelete = editable,
            CanAssign = AccessRules.CanAssign(actor) && isOpenForChanges,
            CanChangeStatus = AccessRules.CanChangeStatus(request, actor) && isOpenForChanges,
            CanComment = AccessRules.CanView(request, actor) && isOpenForChanges
        };
    }

    // Saves the reporter's changes while the request is still open and unassigned.
    public async Task<ServiceResult> UpdateAsync(int id, RequestInput input, User actor)
    {
        var request = await _requests.GetByIdAsync(id);
        if (request is null)
            return ServiceResult.NotFound(Messages.RequestNotFound);

        if (request.ReporterId != actor.Id)
            return ServiceResult.Forbidden();

        if (!RequestRules.IsEditable(request))
            return ServiceResult.Invalid(Messages.RequestLocked);

        var check = RequestRules.Validate(input);
        if (!check.Succeeded)
            return check;

        var exists = await CheckCategoryAndCampusAsync(input);
        if (!exists.Succeeded)
            return exists;

        request.Title = input.Title.Trim();
        request.Description = input.Description.Trim();
        request.CategoryId = input.CategoryId;
        request.CampusId = input.CampusId;
        request.Location = input.Location.Trim();
        request.Priority = input.Priority;
        request.UpdatedAt = Now();

        await _requests.UpdateAsync(request);
        return ServiceResult.Success();
    }

    // Deletes the reporter's request while it's still open and unassigned.
    public async Task<ServiceResult> DeleteAsync(int id, User actor)
    {
        var request = await _requests.GetByIdAsync(id);
        if (request is null)
            return ServiceResult.NotFound(Messages.RequestNotFound);

        if (request.ReporterId != actor.Id)
            return ServiceResult.Forbidden();

        if (!RequestRules.IsEditable(request))
            return ServiceResult.Invalid(Messages.RequestLocked);

        await _requests.DeleteAsync(request);
        return ServiceResult.Success();
    }

    // The chosen category and campus must exist in the database.
    private async Task<ServiceResult> CheckCategoryAndCampusAsync(RequestInput input)
    {
        if (await _lookups.GetCategoryAsync(input.CategoryId) is null)
            return ServiceResult.Invalid(Messages.CategoryNotFound, nameof(RequestInput.CategoryId));

        if (await _lookups.GetCampusAsync(input.CampusId) is null)
            return ServiceResult.Invalid(Messages.CampusNotFound, nameof(RequestInput.CampusId));

        return ServiceResult.Success();
    }

    // The current time in UTC, from the injected TimeProvider, so tests can fix it.
    private DateTime Now()
    {
        return _time.GetUtcNow().UtcDateTime;
    }
}