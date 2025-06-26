using E_Contract.Service.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        //private readonly IImageService _imageService;

        //public ImageController(IImageService imageService)
        //{
        //    _imageService = imageService;
        //}

        //[HttpPost("detect-plate")]
        //public async Task<IActionResult> DetectPlate([FromForm] IFormFile image)
        //{
        //    using var ms = new MemoryStream();
        //    await image.CopyToAsync(ms);
        //    var result = await _imageService.DetectLicensePlateAsync(ms.ToArray());
        //    return Ok(new { plate = result });
        //}

        //[HttpPost("face-id")]
        //public async Task<IActionResult> RecognizeFace([FromForm] IFormFile image)
        //{
        //    using var ms = new MemoryStream();
        //    await image.CopyToAsync(ms);
        //    var result = await _imageService.RecognizeFaceAsync(ms.ToArray());
        //    return Ok(new { identity = result });
        //}
    }
}
