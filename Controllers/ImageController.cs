using Microsoft.AspNetCore.Mvc;
using SocialMedia.Models.Dto.Request;
using SocialMedia.Services;

namespace SocialMedia.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ImageController : ControllerBase
  {
    private readonly IImageService _imageService;
    public ImageController(IImageService imageService)
    {
      _imageService = imageService;
    }
    [HttpPost("Upload")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var response = await _imageService.UploadImage(request);
      if (response.Status == 201)
      {
        return CreatedAtAction(nameof(UploadImage), new { imageUrl = response.Data }, response);
      }
      return StatusCode(response.Status, response);
    }
    [HttpGet("Download")]
    public async Task<IActionResult> DownloadImage([FromQuery] string imageUrl)
    {
      if (string.IsNullOrEmpty(imageUrl))
      {
        return BadRequest("URL không hợp lệ!");
      }
      var response = await _imageService.DownloadImage(imageUrl);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteImage([FromQuery] string imageUrl)
    {
      if (string.IsNullOrEmpty(imageUrl))
      {
        return BadRequest("URL không hợp lệ!");
      }
      var response = await _imageService.DeleteImage(imageUrl);
      if (response.Status == 200)
      {
        return Ok(response);
      }
      return StatusCode(response.Status, response);
    }
  }
}