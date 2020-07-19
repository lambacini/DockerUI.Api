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
    public class VolumeController : BaseApiController
    {
        private ILogger<VolumeController> _logger;
        private ContainerService _service;

        public VolumeController(ILogger<VolumeController> logger, ContainerService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
