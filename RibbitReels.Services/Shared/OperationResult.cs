using System.Net;

namespace RibbitReels.Services.Shared;

public class OperationResult<T>
{
    public T Value => GetValue();

    public readonly int StatusCode;
    public Exception Exception => GetException();
    public bool IsSuccessful { get; }
    public string? FailureMessage { get; }
    
    private readonly T? _value;
    private readonly Exception? _exception;

    public static OperationResult<T> Success(T result, HttpStatusCode? statusCode = null) =>
        new(result, statusCode);
    
    public static OperationResult<T> Fail(string message, HttpStatusCode? statusCode = null) =>
        new(message, statusCode);
    
    public static OperationResult<T> Fail(
        Exception exception,
        string? message = null,
        HttpStatusCode? statusCode = null
    ) => new(exception, message, statusCode);
    
    private OperationResult(T value, HttpStatusCode? statusCode)
    {
        _value = value;
        _exception = null;
        StatusCode = GetStatusCode(statusCode ?? HttpStatusCode.OK);
        IsSuccessful = true;
        FailureMessage = null;
    }
    
    private OperationResult(string message, HttpStatusCode? statusCode)
    {
        _value = default;
        _exception = null;
        StatusCode = GetStatusCode(statusCode ?? HttpStatusCode.InternalServerError);
        IsSuccessful = false;
        FailureMessage = message;
    }
    
    private OperationResult(Exception exception, string? message, HttpStatusCode? statusCode)
    {
        _value = default;
        _exception = exception;
        StatusCode = GetStatusCode(statusCode ?? HttpStatusCode.InternalServerError);
        IsSuccessful = false;
        FailureMessage = message ?? exception.Message;
    }

    private static int GetStatusCode(HttpStatusCode statusCode) => (int)statusCode;

    private T GetValue()
    {
        if (_value is null)
        {
            throw new InvalidOperationException("Value is not available!");
        }

        return _value;
    }

    private Exception GetException()
    {
        if (_exception is null)
        {
            throw new InvalidOperationException("Exception is not available!");
        }

        return _exception;
    }
}