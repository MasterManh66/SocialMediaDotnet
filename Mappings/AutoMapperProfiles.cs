using AutoMapper;
using SocialMedia.Models.Dto.Comment;
using SocialMedia.Models.Dto.Friend;
using SocialMedia.Models.Dto.Like;
using SocialMedia.Models.Dto.Post;
using SocialMedia.Models.Dto.User;
using SocialMedia.Models.Entities;

namespace SocialMedia.Mappings
{
  public class AutoMapperProfiles : Profile
  {
    public AutoMapperProfiles() 
    {
      CreateMap<User, UserDto>();
      CreateMap<Comment, CommentDto>();
      CreateMap<AddCommentRequestDto, Comment>();
      CreateMap<AddPostRequestDto, Post>();
      CreateMap<Post, PostDto>();
      CreateMap<AddLikeRequestDto, Like>();
      CreateMap<Like, LikeDto>();
      CreateMap<Friend, FriendDto>();
    }
  }
}
