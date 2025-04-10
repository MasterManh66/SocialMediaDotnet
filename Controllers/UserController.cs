using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Services.Users;

namespace SocialMedia.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpPost("register a new user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
      var response = await _userService.RegisterUser(request);
      if (response.Status == 201)
      {
        return CreatedAtAction(nameof(RegisterUser), new { email = request.Email }, response);
      }
      return StatusCode(response.Status, response);
    }
  }
}
