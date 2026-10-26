using CAMPUS.BLL.Common;
using CAMPUS.BLL.Services;
using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Repositories;
using Moq;

namespace CAMPUS.Tests.Requests;

public class CommentServiceTests
{
    private readonly Mock<IServiceRequestRepository> _requests = new();
    private readonly Mock<ICommentRepository> _comments = new();
    private readonly FakeTimeProvider _time = new(DateTimeOffset.Parse("2026-01-10T10:00:00Z"));

    [Fact]
    public async Task GetForRequestAsync_RequestMissing_ReturnsNotFound()
    {
        _requests.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((ServiceRequest?)null);
        var service = NewService();

        var result = await service.GetForRequestAsync(10, Reporter());

        Assert.Equal(ResultKind.NotFound, result.Kind);
        Assert.Equal(Messages.RequestNotFound, result.Error);
    }

    [Fact]
    public async Task GetForRequestAsync_Reporter_ReturnsOrderedComments()
    {
        _requests.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(NewRequest(reporterId: 3, teamId: 2));
        _comments.Setup(c => c.GetByRequestAsync(10)).ReturnsAsync(new List<Comment>
        {
            new() { Id = 1, Text = "first" },
            new() { Id = 2, Text = "second" }
        });
        var service = NewService();

        var result = await service.GetForRequestAsync(10, Reporter());

        Assert.True(result.Succeeded);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task AddAsync_ResolvedRequest_ReturnsCommentsClosed()
    {
        _requests.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(NewRequest(status: RequestStatus.Resolved, reporterId: 3, teamId: 2));
        var service = NewService();

        var result = await service.AddAsync(10, "note", Reporter());

        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.CommentsClosed, result.Error);
    }

    [Fact]
    public async Task AddAsync_BlankComment_ReturnsFieldError()
    {
        _requests.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(NewRequest(reporterId: 3, teamId: 2));
        var service = NewService();

        var result = await service.AddAsync(10, " ", Reporter());

        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal("Text", result.Field);
    }

    [Fact]
    public async Task AddAsync_ValidComment_SavesTrimmedComment()
    {
        _requests.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(NewRequest(reporterId: 3, teamId: 2));
        var service = NewService();

        var result = await service.AddAsync(10, "  hello  ", Reporter());

        Assert.True(result.Succeeded);
        _comments.Verify(c => c.AddAsync(It.Is<Comment>(x =>
            x.ServiceRequestId == 10
            && x.AuthorId == 3
            && x.Text == "hello"
            && x.CreatedAt == _time.GetUtcNow().UtcDateTime)), Times.Once);
    }

    private CommentService NewService() => new(_requests.Object, _comments.Object, _time);

    private static ServiceRequest NewRequest(
        RequestStatus status = RequestStatus.Open,
        int reporterId = 3,
        int? teamId = null)
    {
        return new ServiceRequest
        {
            Id = 10,
            Status = status,
            ReporterId = reporterId,
            AssignedTeamId = teamId
        };
    }

    private static User Reporter() => new() { Id = 3, Role = Roles.Employee };

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
