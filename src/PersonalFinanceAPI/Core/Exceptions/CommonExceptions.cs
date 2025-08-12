namespace PersonalFinanceAPI.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public NotFoundException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ValidationException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public ValidationException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the ValidationException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when trying to create a resource that already exists
/// </summary>
public class DuplicateResourceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the DuplicateResourceException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public DuplicateResourceException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the DuplicateResourceException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public DuplicateResourceException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when user lacks sufficient permissions
/// </summary>
public class InsufficientPermissionsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the InsufficientPermissionsException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public InsufficientPermissionsException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the InsufficientPermissionsException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public InsufficientPermissionsException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown for bad requests
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the BadRequestException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public BadRequestException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the BadRequestException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a resource conflict occurs
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ConflictException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public ConflictException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the ConflictException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public ConflictException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when access is forbidden
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ForbiddenException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public ForbiddenException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the ForbiddenException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown for business logic violations
/// </summary>
public class BusinessLogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the BusinessLogicException class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public BusinessLogicException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the BusinessLogicException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public BusinessLogicException(string message, Exception innerException) : base(message, innerException) { }
}
