namespace SocialMedia.Models.Dto.Response
{
  public class UserReportResponse
  {
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public int TotalPosts { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int NewFriends { get; set; }
  }
}
