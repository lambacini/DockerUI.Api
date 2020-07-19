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
    public class ContainerController : BaseApiController
    {
        private ILogger<DockerController> _logger;
        private DockerService _service;

        public ContainerController(ILogger<DockerController> logger, DockerService service)
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

        [HttpPost]
        [Route("Start")]
        public async Task<ActionResult> StartContainer(string containerid)
        {
            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);

            if (container == null)
                return ErrorResponse("$Container {containerid] not found").ToBadRequest();

            var result = await _service.StartContainer(containerid);

            return SuccessResponse(result).ToOk();
        }

        [HttpPost]
        [Route("Stop")]
        public async Task<ActionResult> StopContainer(string containerid)
        {
            
            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);

            if (container == null)
                return ErrorResponse("$Container {containerid] not found").ToBadRequest();

            var result = await _service.StopContainer(containerid);

            return SuccessResponse(result).ToOk();
        }

        [HttpPost]
        [Route("Rename")]
        public async Task<ActionResult> RenameContainer(string containerid,string newName){
            var result = await _service.RenameContainer(containerid,newName);

            return SuccessResponse(result).ToOk();
        }

        [HttpDelete]
        [Route("Remove")]
        public async Task<ActionResult> RemoveContainer(string containerid){
            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);

            if (container == null)
                return ErrorResponse("$Container {containerid] not found").ToBadRequest();

            var result = await _service.RemoveContainer(containerid);

            return SuccessResponse(result).ToOk();
        }
    }
}
