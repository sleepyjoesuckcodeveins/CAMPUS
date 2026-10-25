using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Repositories;

namespace CAMPUS.BLL.Models;

/// The numbers and the latest requests on the homepage.
public class DashboardStats
{
    public RequestCounts Counts { get; set; } = new();

    /// At most 5 requests, newest first.
    public List<ServiceRequest> Latest { get; set; } = [];
}