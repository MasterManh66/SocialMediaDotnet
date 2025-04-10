namespace SocialMedia.Models.Dto.Response
{
  public class ApiResponse<T>
  {
    public int Status { get; set; }

    public string Message { get; set; }

    public T? Data { get; set; }

    public DateTime TimeStamp { get; set; }

    public ApiResponse(int status, string message, T? data)
    {
      Status = status;
      Message = message;
      Data = data;
      TimeStamp = DateTime.UtcNow;
    }
    public ApiResponse<T> Success(T? data)
    {
      return new ApiResponse<T>(200, "Success", data);
    }
    public ApiResponse<T> Error(string message)
    {
      return new ApiResponse<T>(400, message, default);
    }
  }
}
