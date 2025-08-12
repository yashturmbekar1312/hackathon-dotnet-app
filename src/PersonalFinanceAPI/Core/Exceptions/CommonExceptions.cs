namespace PersonalFinanceAPI.Core.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}

public class DuplicateResourceException : Exception
{
    public DuplicateResourceException(string message) : base(message) { }
    public DuplicateResourceException(string message, Exception innerException) : base(message, innerException) { }
}

public class InsufficientPermissionsException : Exception
{
    public InsufficientPermissionsException(string message) : base(message) { }
    public InsufficientPermissionsException(string message, Exception innerException) : base(message, innerException) { }
}
