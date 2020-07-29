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
    public class ImagesService
    {
        private ILogger<DockerService> _logger;
        private DockerClient _client;

        public ImagesService(ILogger<DockerService> logger)
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

        public async Task<IList<ImagesListResponse>> GetImages()
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters());

            return images;
        }

        public async Task<IList<IDictionary<string, string>>> RemoveImage(string name, bool force = false, bool prune = false)
        {
            var result = await _client.Images.DeleteImageAsync(name, new ImageDeleteParameters
            {
                Force = force
            });

            return result;
        }

        public async Task<IList<ImageSearchResponse>> SearchImage(string name)
        {
            var result = await _client.Images.SearchImagesAsync(new ImagesSearchParameters
            {
                Term = name
            });

            return result;
        }
    }
}
