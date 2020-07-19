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
    public class ContainerService
    {
        private ILogger<DockerService> _logger;
        private DockerClient _client;

        public ContainerService(ILogger<DockerService> logger)
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

        public async Task<IList<ContainerListResponse>> GetContainerLists(int limit = 20)
        {
            IList<ContainerListResponse> containers = await _client.Containers.ListContainersAsync(
                    new ContainersListParameters()
                    {
                        Limit = limit,
                    });

            return containers;
        }

        public async Task<CreateContainerResponse> CreateContainer(bool removeIfExists = true)
        {
            try
            {
                if (removeIfExists)
                {
                    var containers = await GetContainerLists(100);
                    var old = containers.FirstOrDefault(p => p.Names.Contains("/TestClient"));
                    if (old != null)
                        await _client.Containers.RemoveContainerAsync(old.ID, new ContainerRemoveParameters());
                }

                var response = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = "lambacini/forticlient",

                    Env = new List<string>
                {
                    "PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin",
                    "DESTIP=10.63.10.8",
                    "DESTPORT=3389",
                    "RECONNECT=TRUE",
                    "VPNADDR=88.247.3.3:10443",
                    "VPNUSER=mcini",
                    "VPNPASS=^^Lamba9k3x0663",
                    "SOURCEPORT=3380"
                },
                    Name = "TestClient",
                    ExposedPorts = new Dictionary<string, EmptyStruct> { { "3389", new EmptyStruct() } },
                    HostConfig = new HostConfig
                    {
                        Privileged = true,
                        PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "3389", new List<PortBinding> { new PortBinding { HostIP = "localhost", HostPort = "3380" } } }
                    },
                        PublishAllPorts = true
                    }
                });

                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> StartContainer(string containerid)
        {
            var result = await _client.Containers.StartContainerAsync(containerid, null);

            return result;
        }

        public async Task<bool> StopContainer(string containerid)
        {
            var result = await _client.Containers.StopContainerAsync(containerid, new ContainerStopParameters());

            return result;
        }

        public async Task<bool> RemoveContainer(string containerid)
        {
            await _client.Containers.RemoveContainerAsync(containerid, new ContainerRemoveParameters());

            return true;
        }

        public async Task<bool> RenameContainer(string containerid, string newName)
        {
            await _client.Containers.RenameContainerAsync(containerid, new ContainerRenameParameters()
            {
                NewName = newName
            }, new System.Threading.CancellationToken());

            return true;
        }
    }
}
