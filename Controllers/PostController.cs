using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Models.Dto.Request;
using SocialMedia.Services;

namespace SocialMedia.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class PostController : ControllerBase
  {
    private readonly IPostService _postService;
    public PostController(IPostService postService)
    {
      _postService = postService;
    }
    [Authorize]
    [HttpPost("CreatPost")]
    public async Task<IActionResult> CreatePost([FromForm] PostCreateRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _postService.CreatePost(request);
      if (response.Status == 201)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpPut("UpdatePost")]
    public async Task<IActionResult> UpdatePost([FromForm] PostEditRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _postService.UpdatePost(request);
      if (response.Status != 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpGet("GetPostUser")]
    public async Task<IActionResult> GetPost()
    {
      var response = await _postService.GetPost();
      if (response.Status != 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpDelete("DeletePost")]
    public async Task<IActionResult> DeletePost([FromQuery] int postId)
    {
      var response = await _postService.DeletePost(postId);
      if (response.Status != 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpGet("SearchPost")]
    public async Task<IActionResult> SearchPost([FromQuery] string keyWord)
    {
      var response = await _postService.SearchPost(keyWord);
      if (response.Status != 200)
      { 
        return Ok(response); 
      }
      return StatusCode(response.Status, response);
    }
  }
}