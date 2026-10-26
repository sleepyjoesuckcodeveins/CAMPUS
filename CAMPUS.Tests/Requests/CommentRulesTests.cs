using CAMPUS.BLL.Common;
using CAMPUS.BLL.Rules;

namespace CAMPUS.Tests.Requests;

public class CommentRulesTests
{
    [Fact]
    public void Validate_ValidComment_ReturnsSuccess()
    {
        var result = CommentRules.Validate("Looks good");

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_EmptyComment_ReturnsRequiredMessage()
    {
        var result = CommentRules.Validate("   ");

        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.CommentRequired, result.Error);
        Assert.Equal("Text", result.Field);
    }

    [Fact]
    public void Validate_TooLongComment_ReturnsTooLongMessage()
    {
        var result = CommentRules.Validate(new string('x', 1001));

        Assert.Equal(ResultKind.Invalid, result.Kind);
        Assert.Equal(Messages.CommentTooLong, result.Error);
        Assert.Equal("Text", result.Field);
    }
}
