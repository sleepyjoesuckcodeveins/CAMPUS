using CAMPUS.BLL.Common;

namespace CAMPUS.BLL.Rules;

public static class CommentRules
{
    private const int TextMaxLength = 1000;

    public static ServiceResult Validate(string? text)
    {
        var value = (text ?? string.Empty).Trim();
        if (value.Length == 0)
            return ServiceResult.Invalid(Messages.CommentRequired, "Text");

        if (value.Length > TextMaxLength)
            return ServiceResult.Invalid(Messages.CommentTooLong, "Text");

        return ServiceResult.Success();
    }
}
