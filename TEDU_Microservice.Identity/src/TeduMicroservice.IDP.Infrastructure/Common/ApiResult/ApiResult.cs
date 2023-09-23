using Microsoft.AspNetCore.Mvc;

namespace TeduMicroservice.IDP.Infrastructure.Common.ApiResult;
public class ApiResult<T> : IActionResult
{
    public string Message { get; set; }
    public bool IsSucceeded { get; set; }
    public T Result { get; set; }

    public ApiResult()
    {

    }

    public ApiResult(bool isSucceeded, string? message = null)
    {
        Message = message;
        IsSucceeded = isSucceeded;
    }

    public ApiResult(bool isSucceeded,T result, string? message = null)
    {
        Message = message;
        IsSucceeded = isSucceeded;
        Result = result;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(this);

        await objectResult.ExecuteResultAsync(context);
    }
}
