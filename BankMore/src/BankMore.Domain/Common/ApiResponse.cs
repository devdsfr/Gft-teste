namespace BankMore.Domain.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorType { get; set; }

    public static ApiResponse<T> SuccessResult(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResult(string errorMessage, string errorType)
    {
        return new ApiResponse<T>
        {
            Success = false,
            ErrorMessage = errorMessage,
            ErrorType = errorType
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResult()
    {
        return new ApiResponse
        {
            Success = true
        };
    }

    public static new ApiResponse ErrorResult(string errorMessage, string errorType)
    {
        return new ApiResponse
        {
            Success = false,
            ErrorMessage = errorMessage,
            ErrorType = errorType
        };
    }
}

