using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DockerUI.Api.Services
{
    public class SwarmService
    {
        private ILogger<SwarmService> _logger;
        private DockerClient _client;

        public SwarmService(ILogger<SwarmService> logger)
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

        public async Task<string> InitSwarm(SwarmInitParameters swarmPrm)
        {
            var result = await _client.Swarm.InitSwarmAsync(swarmPrm);

            return result;
        }

        public async Task<IEnumerable<NodeListResponse>> ListNodes(string nodeid)
        {
            var result = await _client.Swarm.ListNodesAsync();

            return result;
        }

        public async Task<NodeListResponse> InspectNode(string nodeid)
        {
            var result = await _client.Swarm.InspectNodeAsync(nodeid);

            return result;
        }
    }
}
