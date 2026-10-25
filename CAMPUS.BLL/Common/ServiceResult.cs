namespace CAMPUS.BLL.Common;

/// <summary>
/// The result of a service action. A page handles every kind as in the table in C4.
/// </summary>
public class ServiceResult
{
    protected ServiceResult(ResultKind kind, string? error, string? field)
    {
        Kind = kind;
        Error = error;
        Field = field;
    }

    public bool Succeeded => Kind == ResultKind.Success;

    public ResultKind Kind { get; }

    /// <summary>
    /// A text from Messages, or null on success and on Forbidden.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// The name of the property the error belongs to, or null when the error is about the whole request.
    /// </summary>
    public string? Field { get; }

    public static ServiceResult Success() => new(ResultKind.Success, null, null);

    public static ServiceResult Invalid(string error, string? field = null) => new(ResultKind.Invalid, error, field);

    public static ServiceResult NotFound(string error) => new(ResultKind.NotFound, error, null);

    public static ServiceResult Forbidden() => new(ResultKind.Forbidden, null, null);
}

/// <summary>
/// The same as ServiceResult, plus the value on success.
/// </summary>
public class ServiceResult<T> : ServiceResult
{
    private ServiceResult(ResultKind kind, T? value, string? error, string? field) : base(kind, error, field)
    {
        Value = value;
    }

    public T? Value { get; }

    public static ServiceResult<T> Success(T value) => new(ResultKind.Success, value, null, null);

    public static new ServiceResult<T> Invalid(string error, string? field = null) => new(ResultKind.Invalid, default, error, field);

    public static new ServiceResult<T> NotFound(string error) => new(ResultKind.NotFound, default, error, null);

    public static new ServiceResult<T> Forbidden() => new(ResultKind.Forbidden, default, null, null);
}