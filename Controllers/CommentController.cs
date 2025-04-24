using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Models.Dto.Comment;
using SocialMedia.Services;

namespace SocialMedia.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CommentController : ControllerBase
  {
    private readonly ICommentService _commentService;
    public CommentController(ICommentService commentService)
    {
      _commentService = commentService;
    }
    [Authorize]
    [HttpPost("CommentPost")]
    public async Task<IActionResult> CommentPost([FromForm] AddCommentRequestDto request)
    {
      var response = await _commentService.CommentPost(request);
      if (response.Status != 201)
      {
        return StatusCode(response.Status, response);
      }
      return Ok(response);
    }
    [Authorize]
    [HttpGet("CommentsOfUser")]
    public async Task<IActionResult> CommentsOfUser()
    {
      var response = await _commentService.CommentsOfUser();
      if (response.Status != 200)
      {
        return StatusCode(response.Status, response);
      }
      return Ok(response);
    }
    [Authorize]
    [HttpPut("EditComment")]
    public async Task<IActionResult> EditComment([FromForm] UpdateCommentRequestDto request)
    {
      var response = await _commentService.EditComment(request);
      if (response.Status != 200)
      {
        return StatusCode(response.Status, response);
      }
      return Ok(response);
    }
    [Authorize]
    [HttpDelete("DeleteComment")]
    public async Task<IActionResult> DeleteComment([FromQuery] int commentId)
    {
      var response = await _commentService.DeleteComment(commentId);
      if (response.Status != 204)
      {
        return StatusCode(response.Status, response);
      }
      return Ok(response);
    }
  }
}
