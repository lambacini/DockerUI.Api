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
    public class DockerController : BaseApiController
    {
        private ILogger<DockerController> _logger;
        private DockerService _service;

        public DockerController(ILogger<DockerController> logger,DockerService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var containers = await _service.GetContainerLists(100);
            return SuccessResponse(containers).ToOk();
        }

        [HttpGet]
        [Route("{containerid}")]
        public async Task<ActionResult> GetById(string containerid)
        {
            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);
            if (container == null)
                return ErrorResponse($"Container {containerid} not found").ToBadRequest();

            return SuccessResponse(container).ToOk();
        }
    }
}
