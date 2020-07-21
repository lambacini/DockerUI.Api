using DockerUI.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DockerUI.Api.Controllers
{
    [ApiController]
    public class ImageController : BaseApiController
    {
        private ILogger<ImageController> _logger;
        private ImagesService _service;

        public ImageController(ILogger<ImageController> logger, ImagesService service)
        {
            _logger = logger;
            _service = service;
        }

        
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var images = await _service.GetImages();
            return SuccessResponse(images).ToOk();
        }

    }
}
