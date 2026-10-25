namespace CAMPUS.BLL.Models;

/// What the logged-in user may do with one request. Pages use it to show or hide buttons.
public class RequestPermissions
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanAssign { get; set; }
    public bool CanChangeStatus { get; set; }
    public bool CanComment { get; set; }
}