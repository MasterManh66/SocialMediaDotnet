using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Models.Dto.Like;
using SocialMedia.Services;

namespace SocialMedia.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class LikeController : ControllerBase
  {
    private readonly ILikeService _likeService;
    public LikeController(ILikeService likeService)
    {
      _likeService = likeService;
    }
    [Authorize]
    [HttpPost("LikePost")]
    public async Task<IActionResult> LikePost([FromQuery] AddLikeRequestDto request)
    {
      var response = await _likeService.LikePost(request);
      if(response.Status == 201)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpGet("LikeOfUser")]
    public async Task<IActionResult> LikeOfUser()
    {
      var response = await _likeService.LikeOfUser();
      if (response.Status != 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpDelete("UnlikePost")]
    public async Task<IActionResult> UnlikePost([FromQuery] AddLikeRequestDto request)
    {
      var response = await _likeService.UnlikePost(request);
      if (response.Status != 201)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
  }
}
