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
  }
}
