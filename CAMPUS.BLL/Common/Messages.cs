namespace CAMPUS.BLL.Common;

/// Every text a user can read after an action (C4). Always use these constants, never type the text yourself.
/// {0} and {1} are filled in with string.Format.
public static class Messages
{
    public const string TitleRequired = "Enter a title.";
    public const string TitleTooLong = "The title can be at most 100 characters.";
    public const string DescriptionRequired = "Describe the problem.";
    public const string DescriptionTooLong = "The description can be at most 1000 characters.";
    public const string CategoryRequired = "Choose a category.";
    public const string CampusRequired = "Choose a campus.";
    public const string LocationRequired = "Enter the location, for example the building and room.";
    public const string LocationTooLong = "The location can be at most 100 characters.";
    public const string PriorityRequired = "Choose a priority.";
    public const string CommentRequired = "Write a comment first.";
    public const string CommentTooLong = "A comment can be at most 1000 characters.";
    public const string TeamRequired = "Choose a team.";
    public const string StatusRequired = "Choose the new status.";

    public const string RequestNotFound = "This request doesn't exist or has been deleted.";
    public const string UserNotFound = "This user doesn't exist.";
    public const string CategoryNotFound = "Choose a category from the list.";
    public const string CampusNotFound = "Choose a campus from the list.";
    public const string TeamNotFound = "Choose a team from the list.";

    public const string RequestLocked = "You can only change or delete a request while it's open and not assigned yet.";
    public const string RequestAlreadyResolved = "This request is resolved and can't be changed anymore.";
    public const string AlreadyAssigned = "This request is already assigned to this team.";
    public const string SameStatus = "The request already has this status.";
    public const string StatusChangeNotAllowed = "This status change isn't allowed.";
    public const string CommentsClosed = "You can't comment on a resolved request.";
    public const string NotSupportMember = "Only support team members can be linked to a team.";

    public const string RequestCreated = "Your request {0} has been submitted.";

    public const string RequestUpdated = "Your changes have been saved.";

    public const string RequestDeleted = "Your request {0} has been deleted.";

    public const string RequestAssigned = "{0} has been assigned to the {1} team.";

    public const string StatusChanged = "{0} is now {1}.";

    public const string CommentAdded = "Your comment has been added.";

    public const string TeamLinked = "{0} is now in the {1} team.";

    public const string TeamUnlinked = "{0} is no longer in a team.";

    public const string MyRequestsEmpty = "You haven't submitted any requests yet.";
    public const string AllRequestsEmpty = "No requests match these filters.";
    public const string AssignedEmpty = "There are no requests for your team.";
    public const string NoTeam = "You aren't linked to a support team yet. Ask the administrator to add you to a team.";
    public const string CommentsEmpty = "No comments yet.";
    public const string LatestEmpty = "There are no requests yet.";
    public const string DeleteConfirm = "Are you sure you want to delete this request? This can't be undone.";
    public const string AccessDenied = "You don't have permission to open this page.";
}