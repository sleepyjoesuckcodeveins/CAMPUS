using CAMPUS.BLL.Common;
using CAMPUS.BLL.Models;
using CAMPUS.BLL.Rules;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.Tests.Requests;

public class RequestRulesTests
{
    // A valid input; each test changes only the field it's about.
    private static RequestInput ValidInput() => new()
    {
        Title = "Water leak in north hallway",
        Description = "Water is dripping from the ceiling near the north stairs.",
        CategoryId = 1,
        CampusId = 1,
        Location = "De Lotus, north hallway",
        Priority = Priority.High
    };

    [Fact]
    public void IsEditable_OpenUnassigned_ReturnsTrue()
    {
        // Arrange
        var request = new ServiceRequest { Status = RequestStatus.Open, AssignedTeamId = null };

        // Act
        var result = RequestRules.IsEditable(request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsEditable_Assigned_ReturnsFalse()
    {
        // Arrange
        var request = new ServiceRequest { Status = RequestStatus.Open, AssignedTeamId = 2 };

        // Act
        var result = RequestRules.IsEditable(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsEditable_InProgress_ReturnsFalse()
    {
        // Arrange
        var request = new ServiceRequest { Status = RequestStatus.InProgress, AssignedTeamId = null };

        // Act
        var result = RequestRules.IsEditable(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var input = ValidInput();

        // Act
        var result = RequestRules.Validate(input);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_TitleOnlySpaces_ReturnsTitleRequired()
    {
        // Arrange
        var input = ValidInput();
        input.Title = "   ";

        // Act
        var result = RequestRules.Validate(input);

        // Assert
        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.TitleRequired, result.Error);
        Assert.Equal(nameof(RequestInput.Title), result.Field);
    }

    [Fact]
    public void Validate_Title101Characters_ReturnsTitleTooLong()
    {
        // Arrange
        var input = ValidInput();
        input.Title = new string('a', 101);

        // Act
        var result = RequestRules.Validate(input);

        // Assert
        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.TitleTooLong, result.Error);
        Assert.Equal(nameof(RequestInput.Title), result.Field);
    }

    [Fact]
    public void Validate_CategoryZero_ReturnsCategoryRequired()
    {
        // Arrange
        var input = ValidInput();
        input.CategoryId = 0;

        // Act
        var result = RequestRules.Validate(input);

        // Assert
        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.CategoryRequired, result.Error);
        Assert.Equal(nameof(RequestInput.CategoryId), result.Field);
    }

    [Fact]
    public void Validate_UndefinedPriority_ReturnsPriorityRequired()
    {
        // Arrange
        var input = ValidInput();
        input.Priority = (Priority)99;

        // Act
        var result = RequestRules.Validate(input);

        // Assert
        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.PriorityRequired, result.Error);
        Assert.Equal(nameof(RequestInput.Priority), result.Field);
    }

    [Fact]
    public void Validate_TitleAndLocationEmpty_ReturnsTitleFirst()
    {
        // Arrange
        var input = ValidInput();
        input.Title = "";
        input.Location = "";

        // Act
        var result = RequestRules.Validate(input);

        // Assert
        Assert.Equal(Messages.TitleRequired, result.Error);
        Assert.Equal(nameof(RequestInput.Title), result.Field);
    }
}