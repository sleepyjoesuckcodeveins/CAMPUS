using CAMPUS.DAL.Repositories;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.DAL.Tests.Repositories;

public class CommentRepositoryTests
{
    [Fact]
    public async Task AddAsync_SavesComment()
    {
        using var db = new SqliteDatabase();
        var request = await CreateRequestAsync(db);
        var repository = new CommentRepository(db.Context);

        await repository.AddAsync(new Comment
        {
            ServiceRequestId = request.Id,
            AuthorId = 1,
            Text = "New comment",
            CreatedAt = DateTime.UtcNow
        });

        var saved = db.Context.Comments.Single(c => c.ServiceRequestId == request.Id && c.Text == "New comment");
        Assert.Equal(1, saved.AuthorId);
    }

    [Fact]
    public async Task GetByRequestAsync_ReturnsOldestFirstWithAuthor()
    {
        using var db = new SqliteDatabase();
        var request = await CreateRequestAsync(db);
        var created = DateTime.UtcNow;
        db.Context.Comments.AddRange(
            new Comment
            {
                ServiceRequestId = request.Id,
                AuthorId = 1,
                Text = "Second",
                CreatedAt = created.AddMinutes(2)
            },
            new Comment
            {
                ServiceRequestId = request.Id,
                AuthorId = 2,
                Text = "First",
                CreatedAt = created
            });
        await db.Context.SaveChangesAsync();

        var repository = new CommentRepository(db.Context);
        var comments = await repository.GetByRequestAsync(request.Id);

        Assert.Equal(new[] { "First", "Second" }, comments.Select(c => c.Text));
        Assert.All(comments, c => Assert.NotNull(c.Author));
    }

    private static async Task<ServiceRequest> CreateRequestAsync(SqliteDatabase db)
    {
        var request = new ServiceRequest
        {
            Id = 9990,
            Title = "Leak",
            Description = "Leak in ceiling",
            CategoryId = 1,
            CampusId = 1,
            Location = "Room 1",
            Priority = Priority.Medium,
            Status = RequestStatus.Open,
            ReporterId = 1,
            CreatedAt = DateTime.UtcNow
        };
        db.Context.ServiceRequests.Add(request);
        await db.Context.SaveChangesAsync();
        return request;
    }
}
