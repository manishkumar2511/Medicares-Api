namespace Medicares.Application.Contracts.Wrappers;

public class Result : Result<object> 
{
    public Result() { }

    public static Task<Result<T>> SuccessAsync<T>(T data)
    {
        return Task.FromResult(new Result<T> { Succeeded = true, Data = data });
    }
    
    public static Task<Result<T>> SuccessAsync<T>(T data, string message)
    {
        return Task.FromResult(new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } });
    }

    public static Result<T> Success<T>(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static Result<T> Success<T>(T data, string message)
    {
        return new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
    }

    public static new Task<Result> FailAsync(string message)
    {
        return Task.FromResult(new Result { Succeeded = false, Messages = new List<string> { message } });
    }

    public static new Task<Result> FailAsync(List<string> messages)
    {
        return Task.FromResult(new Result { Succeeded = false, Messages = messages });
    }
    
    public static new Result Fail(string message)
    {
        return new Result { Succeeded = false, Messages = new List<string> { message } };
    }

    public static new Result Fail(List<string> messages)
    {
        return new Result { Succeeded = false, Messages = messages };
    }
}

public class Result<T>
{
    public List<string> Messages { get; set; } = new();

    public bool Succeeded { get; set; }

    public T Data { get; set; } = default!;

    public int Code { get; set; }

    public Result() { }
    
    public static Result<T> Fail(string message)
    {
        return new Result<T> { Succeeded = false, Messages = new List<string> { message } };
    }

    public static Result<T> Fail(List<string> messages)
    {
        return new Result<T> { Succeeded = false, Messages = messages };
    }

    public static Task<Result<T>> FailAsync(string message)
    {
        return Task.FromResult(new Result<T> { Succeeded = false, Messages = new List<string> { message } });
    }

    public static Task<Result<T>> FailAsync(List<string> messages)
    {
        return Task.FromResult(new Result<T> { Succeeded = false, Messages = messages });
    }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }
    
    public static Result<T> Success(T data, string message)
    {
        return new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
    }

    public static Task<Result<T>> SuccessAsync(T data)
    {
        return Task.FromResult(new Result<T> { Succeeded = true, Data = data });
    }
    
    public static Task<Result<T>> SuccessAsync(T data, string message)
    {
        return Task.FromResult(new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } });
    }
}
