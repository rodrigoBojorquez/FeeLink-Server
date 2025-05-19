using FeeLink.Infrastructure.Services.Assets;
using FeeLink.Api.Common.Controllers;
using FeeLink.Domain.Common.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeeLink.Api.Controllers;

public class AssetsController(ImageService imageService) : ApiController
{
    [HttpGet("image")]
    [AllowAnonymous]
    public IActionResult GetImage([FromQuery] string p)
    {
        var stream = imageService.Get(p);

        if (stream is null)
        {
            return Problem(Errors.Asset.NotFound);
        }
        
        return File(stream, "image/jpeg");
    }
}