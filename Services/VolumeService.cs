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
    public class VolumeService
    {
        private ILogger<DockerService> _logger;
        private DockerClient _client;

        public VolumeService(ILogger<DockerService> logger)
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

        public async Task<VolumesListResponse> GetVolumes()
        {
            var result = await _client.Volumes.ListAsync();
            return result;
        }

        public async Task<VolumeResponse> CreateVolume(string name, string driver, IDictionary<string, string> labels = null)
        {
            var response = await _client.Volumes.CreateAsync(new VolumesCreateParameters
            {
                Name = name,
                Labels = labels,
                Driver = driver
            });

            return response;
        }

        public async Task<bool> RemoveVolume(string name, bool force = false)
        {
            await _client.Volumes.RemoveAsync(name,force);

            return true;
        }
    }
}
