﻿using SocialMedia.Models.Domain.Enums;

namespace SocialMedia.Models.Dto.Post
{
  public class UpdatePostRequestDto
  {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
    public PostEnum? PostStatus { get; set; }
  }
}
