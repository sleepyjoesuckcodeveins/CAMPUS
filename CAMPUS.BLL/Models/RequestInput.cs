using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.BLL.Models;

/// The fields of the request form, as the services receive them.
public class RequestInput
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int CampusId { get; set; }
    public string Location { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
}