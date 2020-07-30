using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DockerUI.Api.Services
{
    public class NetworkService
    {
        private ILogger<DockerService> _logger;
        private DockerClient _client;

        public NetworkService(ILogger<DockerService> logger)
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

        public async Task<IList<NetworkResponse>> GetNetworks()
        {
            var result = await _client.Networks.ListNetworksAsync();
            return result;
        }

        public async Task<NetworksCreateResponse> CreateNetwork(NetworksCreateParameters createPrm)
        {
            var result = await _client.Networks.CreateNetworkAsync(createPrm);
            return result;
        }

        public async Task<bool> DeleteNetwork(string networkid)
        {
            await _client.Networks.DeleteNetworkAsync(networkid);
            return true;
        }

        public async Task<bool> ConnectNetwork(string networkid, string container)
        {
            await _client.Networks.ConnectNetworkAsync(networkid, new NetworkConnectParameters()
            {
                Container = container
            });

            return true;
        }

        public async Task<bool> DisconnectNetwork(string networkid, string container, bool force)
        {
            await _client.Networks.DisconnectNetworkAsync(networkid, new NetworkDisconnectParameters()
            {
                Container = container,
                Force = force
            });

            return true;
        }

        public async Task<NetworksPruneResponse> Prune()
        {
            var result = await _client.Networks.PruneNetworksAsync(new NetworksDeleteUnusedParameters());
            return result;
        }
    }
}
