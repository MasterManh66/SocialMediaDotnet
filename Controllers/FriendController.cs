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
    [Authorize]
    [HttpGet("FriendReceiver")]
    public async Task<IActionResult> GetFriendsReceiver()
    {
      var response = await _friendService.GetFriendsReceiver();
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpPut("AcceptedFriend")]
    public async Task<IActionResult> AcceptedFriend([FromQuery] int RequestId)
    {
      var response = await _friendService.AcceptedFriend(RequestId);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpGet("FriendOfUser")]
    public async Task<IActionResult> FriendOfUser()
    {
      var response = await _friendService.FriendOfUser();
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpDelete("DeleteFriend")]
    public async Task<IActionResult> DeleteFriend([FromQuery] int FriendId)
    {
      var response = await _friendService.DeleteFriend(FriendId);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpGet("SearchUser")]
    public async Task<IActionResult> SearchFriend([FromQuery] string UserName)
    {
      var response = await _friendService.SearchFriend(UserName);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
  }
}
