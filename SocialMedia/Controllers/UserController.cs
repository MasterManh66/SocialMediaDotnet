using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Models.Dto.User;
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
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequestDto request)
    {
      var response = await _userService.RegisterUser(request);
      if (response.Status == 201)
      {
        return CreatedAtAction(nameof(RegisterUser), new { email = request.Email }, response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("LoginUser")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequestDto request)
    {
      var response = await _userService.LoginUser(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("VerifyOtp")]
    public async Task<IActionResult> VerifyOtp([FromBody] AuthUserRequestDto request)
    {
      var response = await _userService.VerifyOtp(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("ForgetPassword")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto request)
    {
      var response = await _userService.ForgetPassword(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpPost("VerifyForgetPassword")]
    public async Task<IActionResult> VerifyForgetPassword([FromBody] VerifyForgetPasswordRequestDto request)
    {
      var response = await _userService.VerifyForgetPasword(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [Authorize]
    [HttpPut("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
      var response = await _userService.ChangePassword(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [Authorize]
    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUser()
    {
      var response = await _userService.GetUser();
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [Authorize]
    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequestDto request)
    {
      var response = await _userService.UpdateUser(request);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
    [Authorize]
    [HttpGet("ReportOfUser")]
    public async Task<IActionResult> ExportUserReportToExcel()
    {
      var reportData = await _userService.ReportOfUser();
      if (reportData == null || !reportData.Any())
      {
        return NotFound("Người dùng 1 tuần qua không có hoạt động gì mới!");
      }

      var excelFile = _userService.ExportUserReportsToExcel(reportData);
      var fileName = $"UserReport_{DateTime.UtcNow:yyyyMMdd}.xlsx";

      return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
  }
}
