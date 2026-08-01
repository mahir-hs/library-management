namespace LibraryManagement.Application.Common.Exceptions;

public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
    public ApplicationException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class NotFoundException : ApplicationException
{
    public NotFoundException(string resourceName, object? id)
        : base($"{resourceName} with id '{id}' was not found.") { }

    public NotFoundException(string message)
        : base(message) { }
}

public class ValidationException : ApplicationException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

public class ConflictException : ApplicationException
{
    public ConflictException(string message)
        : base(message) { }
}

public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message)
        : base(message) { }
}

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message) { }
}
