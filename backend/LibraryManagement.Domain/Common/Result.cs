namespace LibraryManagement.Domain.Common;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Message { get; protected set; }
    public IEnumerable<string>? Errors { get; protected set; }

    protected Result(bool isSuccess, string? message, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors;
    }

    public static Result Success(string? message = null) =>
        new(true, message, null);

    public static Result Failure(string? message, IEnumerable<string>? errors = null) =>
        new(false, message, errors);

    public static Result Failure(IEnumerable<string>? errors) =>
        new(false, "One or more validation errors occurred.", errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool isSuccess, string? message, T? data, IEnumerable<string>? errors)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string? message = null) =>
        new(true, message, data, null);

    public new static Result<T> Failure(string? message, IEnumerable<string>? errors = null) =>
        new(false, message, default, errors);

    public new static Result<T> Failure(IEnumerable<string>? errors) =>
        new(false, "One or more validation errors occurred.", default, errors);
}