using Microsoft.AspNetCore.Mvc;
using SocialMedia.Models.Dto.Request;
using SocialMedia.Services;

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

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _userService.RegisterUser(request);
      if (response.Status == 201)
      {
        return CreatedAtAction(nameof(RegisterUser), new { email = request.Email }, response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("LoginUser")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _userService.LoginUser(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("VerifyOtp")]
    public async Task<IActionResult> VerifyOtp([FromBody] AuthRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _userService.VerifyOtp(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("ForgetPassword")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _userService.ForgetPassword(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("VerifyForgetPassword")]
    public async Task<IActionResult> VerifyForgetPassword([FromBody] VerifyForgetPasswordRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _userService.VerifyForgetPasword(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
  }
}
