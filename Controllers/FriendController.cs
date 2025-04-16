using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Services;

namespace SocialMedia.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class FriendController : ControllerBase
  {
    private readonly IFriendService _friendService;
    public FriendController(IFriendService friendService)
    {
      _friendService = friendService;
    }
    [Authorize]
    [HttpPost("SendFriend")]
    public async Task<IActionResult> SendFriendRequest([FromQuery] int ReceiverId)
    {
      var response = await _friendService.SendFriendRequest(ReceiverId);
      if (response.Status == 201)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpGet("FriendRequest")]
    public async Task<IActionResult> GetFriendRequest()
    {
      var response = await _friendService.GetFriendsRequest();
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
  }
}
