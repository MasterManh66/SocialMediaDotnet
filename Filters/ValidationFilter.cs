using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialMedia.Models.Dto;

namespace YourProjectName.Filters
{
  public class ValidationFilter : IActionFilter
  {
    private readonly ILogger<ValidationFilter> _logger;

    public ValidationFilter(ILogger<ValidationFilter> logger)
    {
      _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
      if (!context.ModelState.IsValid)
      {
        var errors = context.ModelState.Values
                          .SelectMany(x => x.Errors)
                          .Select(x => x.ErrorMessage)
                          .ToList();

        _logger.LogWarning("Request gửi lên bị lỗi: {@Errors}", errors);

        context.Result = new BadRequestObjectResult(
            new ApiResponse<List<string>>(400, "Request gửi lên bị lỗi", errors)
        );
      }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
  }
}
