using System.ComponentModel.DataAnnotations;
using CAMPUS.BLL.Common;
using CAMPUS.BLL.Models;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.PL.Models;

// What the New request and Edit forms send. The page checks it first; the service checks again.
public class RequestInputModel
{
    [Display(Name = "Title")]
    [Required(ErrorMessage = Messages.TitleRequired)]
    [StringLength(100, ErrorMessage = Messages.TitleTooLong)]
    public string Title { get; set; } = string.Empty;

    // Nullable, so an empty dropdown gives our own message instead of "The value '' is invalid".
    [Display(Name = "Category")]
    [Required(ErrorMessage = Messages.CategoryRequired)]
    public int? CategoryId { get; set; }

    [Display(Name = "Campus")]
    [Required(ErrorMessage = Messages.CampusRequired)]
    public int? CampusId { get; set; }

    [Display(Name = "Location")]
    [Required(ErrorMessage = Messages.LocationRequired)]
    [StringLength(100, ErrorMessage = Messages.LocationTooLong)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Priority")]
    [EnumDataType(typeof(Priority), ErrorMessage = Messages.PriorityRequired)]
    public Priority Priority { get; set; } = Priority.Medium;

    [Display(Name = "Description")]
    [Required(ErrorMessage = Messages.DescriptionRequired)]
    [StringLength(1000, ErrorMessage = Messages.DescriptionTooLong)]
    public string Description { get; set; } = string.Empty;

    // Turns the form into what the service needs, with the texts trimmed.
    public RequestInput ToRequestInput()
    {
        return new RequestInput
        {
            Title = (Title ?? string.Empty).Trim(),
            CategoryId = CategoryId ?? 0,
            CampusId = CampusId ?? 0,
            Location = (Location ?? string.Empty).Trim(),
            Priority = Priority,
            Description = (Description ?? string.Empty).Trim()
        };
    }

    // Fills the form with an existing request, for the Edit page.
    public static RequestInputModel FromRequest(ServiceRequest request)
    {
        return new RequestInputModel
        {
            Title = request.Title,
            CategoryId = request.CategoryId,
            CampusId = request.CampusId,
            Location = request.Location,
            Priority = request.Priority,
            Description = request.Description
        };
    }
}