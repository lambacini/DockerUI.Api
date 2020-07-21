using DockerUI.Api.Dto;
using DockerUI.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerUI.Api.Controllers
{
    [ApiController]
    public class ContainerController : BaseApiController
    {
        private ILogger<ContainerController> _logger;
        private ContainerService _service;

        public ContainerController(ILogger<ContainerController> logger, ContainerService service)
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

        [HttpGet]
        [Route("Logs/{containerid}")]
        public async Task<ActionResult> GetLogs(string containerid)
        {
            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);
            if (container == null)
                return ErrorResponse($"Container {containerid} not found").ToBadRequest();

            LogResult logresult = new LogResult();

            using (var result = await _service.GetLogs(containerid))
            {
                UTF8Encoding encoding = new UTF8Encoding(false);
                using (var reader = new StreamReader(result, encoding))
                {
                    string nextLine;

                    while ((nextLine = await reader.ReadLineAsync()) != null)
                    {
                        logresult.Logs.Add(StringUtils.StripUnicodeCharactersFromString(nextLine));
                    }
                }
            }

            return SuccessResponse(logresult.Logs, "Logs in data").ToOk();
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

            return SuccessResponse($"Container {container.Names[0]} successfully stared").ToOk();
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

            return SuccessResponse($"Container {container.Names[0]} successfully stoped").ToOk();
        }

        [HttpPost]
        [Route("Rename")]
        public async Task<ActionResult> RenameContainer(string containerid, string newName)
        {
            var result = await _service.RenameContainer(containerid, newName);

            return SuccessResponse(result).ToOk();
        }

        [HttpDelete]
        [Route("Remove")]
        public async Task<ActionResult> RemoveContainer(string containerid)
        {
            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);

            if (container == null)
                return ErrorResponse("$Container {containerid] not found").ToBadRequest();

            var result = await _service.RemoveContainer(containerid);

            return SuccessResponse($"Container {container.Names[0]} successfully removed").ToOk();
        }

        [HttpPost]
        [Route("Kill")]
        public async Task<ActionResult> KillContainer(string containerid)
        {

            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);

            if (container == null)
                return ErrorResponse("$Container {containerid] not found").ToBadRequest();

            var result = await _service.KillContainer(containerid);

            return SuccessResponse($"Container {container.Names[0]} killed").ToOk();
        }

        [HttpPost]
        [Route("Pause")]
        public async Task<ActionResult> PauseContainer(string containerid)
        {

            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);

            if (container == null)
                return ErrorResponse("$Container {containerid] not found").ToBadRequest();

            var result = await _service.PauseContainer(containerid);

            return SuccessResponse($"Container {container.Names[0]} successfully paused").ToOk();
        }
    }
}
