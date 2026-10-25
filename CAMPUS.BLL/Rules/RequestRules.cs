using CAMPUS.BLL.Common;
using CAMPUS.BLL.Models;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.BLL.Rules;

// Rules for one request. No database access, so they're easy to test.
public static class RequestRules
{
    private const int TitleMaxLength = 100;
    private const int LocationMaxLength = 100;
    private const int DescriptionMaxLength = 1000;

    // A request can be changed or deleted while it's open and not assigned to a team yet.
    public static bool IsEditable(ServiceRequest request)
    {
        return request.Status == RequestStatus.Open && request.AssignedTeamId == null;
    }

    // Checks the input in form order and returns the first problem, or Success.
    public static ServiceResult Validate(RequestInput input)
    {
        var title = (input.Title ?? string.Empty).Trim();
        if (title.Length == 0)
            return ServiceResult.Invalid(Messages.TitleRequired, nameof(RequestInput.Title));
        if (title.Length > TitleMaxLength)
            return ServiceResult.Invalid(Messages.TitleTooLong, nameof(RequestInput.Title));

        if (input.CategoryId <= 0)
            return ServiceResult.Invalid(Messages.CategoryRequired, nameof(RequestInput.CategoryId));

        if (input.CampusId <= 0)
            return ServiceResult.Invalid(Messages.CampusRequired, nameof(RequestInput.CampusId));

        var location = (input.Location ?? string.Empty).Trim();
        if (location.Length == 0)
            return ServiceResult.Invalid(Messages.LocationRequired, nameof(RequestInput.Location));
        if (location.Length > LocationMaxLength)
            return ServiceResult.Invalid(Messages.LocationTooLong, nameof(RequestInput.Location));

        if (!Enum.IsDefined(input.Priority))
            return ServiceResult.Invalid(Messages.PriorityRequired, nameof(RequestInput.Priority));

        var description = (input.Description ?? string.Empty).Trim();
        if (description.Length == 0)
            return ServiceResult.Invalid(Messages.DescriptionRequired, nameof(RequestInput.Description));
        if (description.Length > DescriptionMaxLength)
            return ServiceResult.Invalid(Messages.DescriptionTooLong, nameof(RequestInput.Description));

        return ServiceResult.Success();
    }
}