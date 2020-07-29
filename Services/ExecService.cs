using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DockerUI.Api.Services
{
    public class ExecService
    {
        private ILogger<ExecService> _logger;
        private DockerClient _client;

        public ExecService(ILogger<ExecService> logger)
        {
            _logger = logger;
            Init();
        }

        private void Init()
        {

            var engineAddress = "unix:///var/run/docker.sock";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                engineAddress = "npipe://./pipe/docker_engine";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                engineAddress = "unix:///var/run/docker.sock";
            }

            _client = new DockerClientConfiguration(new Uri(engineAddress))
                .CreateClient();
        }

        public async Task<MultiplexedStream> StartAndAttachExec(string containerid)
        {
            var result = await _client.Exec.StartAndAttachContainerExecAsync(containerid, false);

            return result;
        }


    }
}
