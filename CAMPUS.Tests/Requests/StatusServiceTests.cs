using CAMPUS.BLL.Common;
using CAMPUS.BLL.Services;
using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Repositories;
using Moq;

namespace CAMPUS.Tests.Requests;

public class StatusServiceTests
{
    private readonly Mock<IServiceRequestRepository> _requests = new();
    private readonly FakeTimeProvider _time = new(DateTimeOffset.Parse("2026-01-10T10:00:00Z"));

    [Fact]
    public void GetNextStatuses_Open_ReturnsInProgressAndResolved()
    {
        var service = new StatusService(_requests.Object, _time);

        var statuses = service.GetNextStatuses(RequestStatus.Open);

        Assert.Equal(new[] { RequestStatus.InProgress, RequestStatus.Resolved }, statuses);
    }

    [Fact]
    public async Task ChangeStatusAsync_ResolvedRequest_ReturnsInvalid()
    {
        var request = NewRequest(status: RequestStatus.Resolved);
        _requests.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(request);
        var service = new StatusService(_requests.Object, _time);

        var result = await service.ChangeStatusAsync(7, RequestStatus.InProgress, Manager());

        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.RequestAlreadyResolved, result.Error);
    }

    [Fact]
    public async Task ChangeStatusAsync_UnsupportedTransition_ReturnsInvalid()
    {
        var request = NewRequest(status: RequestStatus.Open);
        _requests.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(request);
        var service = new StatusService(_requests.Object, _time);

        var result = await service.ChangeStatusAsync(7, RequestStatus.Open, Manager());

        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.SameStatus, result.Error);
    }

    [Fact]
    public async Task ChangeStatusAsync_SupportFromOtherTeam_ReturnsForbidden()
    {
        var request = NewRequest(status: RequestStatus.Open, teamId: 2);
        _requests.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(request);
        var service = new StatusService(_requests.Object, _time);

        var result = await service.ChangeStatusAsync(7, RequestStatus.InProgress, Support(teamId: 4));

        Assert.Equal(ResultKind.Forbidden, result.Kind);
    }

    [Fact]
    public async Task ChangeStatusAsync_ValidTransition_UpdatesStatusAndResolvedAt()
    {
        var request = NewRequest(status: RequestStatus.InProgress, teamId: 2);
        _requests.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(request);
        var service = new StatusService(_requests.Object, _time);

        var result = await service.ChangeStatusAsync(7, RequestStatus.Resolved, Support(teamId: 2));

        Assert.True(result.Succeeded);
        Assert.Equal(RequestStatus.Resolved, request.Status);
        Assert.Equal(_time.GetUtcNow().UtcDateTime, request.UpdatedAt);
        Assert.Equal(_time.GetUtcNow().UtcDateTime, request.ResolvedAt);
        _requests.Verify(r => r.UpdateAsync(request), Times.Once);
    }

    private static ServiceRequest NewRequest(RequestStatus status, int? teamId = null)
    {
        return new ServiceRequest
        {
            Id = 7,
            Status = status,
            AssignedTeamId = teamId
        };
    }

    private static User Manager() => new() { Id = 1, Role = Roles.FacilitiesManager };

    private static User Support(int teamId) => new() { Id = 2, Role = Roles.SupportTeam, TeamId = teamId };

    private sealed class FakeTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _value;

        public FakeTimeProvider(DateTimeOffset value)
        {
            _value = value;
        }

        public override DateTimeOffset GetUtcNow() => _value;
    }
}
