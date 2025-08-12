using System.Text.Json.Serialization;

namespace PersonalFinanceAPI.Models.Common;

/// <summary>
/// Base API response structure for consistent response format
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The data returned by the operation
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Message describing the result of the operation
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional error details if the operation failed
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Correlation ID for tracking requests
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <param name="data">The data to return</param>
    /// <param name="message">Success message</param>
    /// <returns>Successful API response</returns>
    public static ApiResponse<T> SuccessResult(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Creates a failed response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of error details</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    /// <summary>
    /// Creates a failed response with a single error
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="error">Single error detail</param>
    /// <returns>Failed API response</returns>
    public static ApiResponse<T> ErrorResult(string message, string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = new List<string> { error }
        };
    }
}

/// <summary>
/// Non-generic API response for operations that don't return data
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Creates a successful response without data
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>Successful API response</returns>
    public static ApiResponse SuccessResult(string message = "Operation completed successfully")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// Creates a failed response without data
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of error details</param>
    /// <returns>Failed API response</returns>
    public new static ApiResponse ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    /// <summary>
    /// Creates a failed response with a single error
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="error">Single error detail</param>
    /// <returns>Failed API response</returns>
    public new static ApiResponse ErrorResult(string message, string error)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = new List<string> { error }
        };
    }
}

/// <summary>
/// Paginated API response for list operations
/// </summary>
/// <typeparam name="T">Type of items in the list</typeparam>
public class PaginatedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a successful paginated response
    /// </summary>
    /// <param name="data">The paginated data</param>
    /// <param name="pageNumber">Current page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="totalCount">Total number of items</param>
    /// <param name="message">Success message</param>
    /// <returns>Successful paginated API response</returns>
    public static PaginatedApiResponse<T> SuccessResult(
        IEnumerable<T> data, 
        int pageNumber, 
        int pageSize, 
        int totalCount, 
        string message = "Operation completed successfully")
    {
        return new PaginatedApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }

    /// <summary>
    /// Creates a failed paginated response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of error details</param>
    /// <returns>Failed paginated API response</returns>
    public new static PaginatedApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new PaginatedApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// Pagination parameters for API requests
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page (max 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Search query for filtering results
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Sort field name
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Indicates if sorting should be descending
    /// </summary>
    [JsonIgnore]
    public bool IsDescending => SortDirection?.ToLower() == "desc";
}
