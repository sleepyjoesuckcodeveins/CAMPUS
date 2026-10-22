using CAMPUS.DOMAIN.Entities;

namespace CAMPUS.DOMAIN.Repositories;

// Person 3's part of the request repository: one request, my requests, add, update, delete.
public partial interface IServiceRequestRepository
{
	// One request with its category, campus, reporter and team, or null. Stays tracked so it can be changed.
	Task<ServiceRequest?> GetByIdAsync(int id);

	// The reporter's requests with category, campus and team, newest first.
	Task<List<ServiceRequest>> GetByReporterAsync(int reporterId);

	// Adds and saves the request; afterwards request.Id is filled in.
	Task AddAsync(ServiceRequest request);

	// Saves the changes of a request loaded with GetByIdAsync.
	Task UpdateAsync(ServiceRequest request);

	// Removes and saves the request; the database deletes its comments.
	Task DeleteAsync(ServiceRequest request);
}