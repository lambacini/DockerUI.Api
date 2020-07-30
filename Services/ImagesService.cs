using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
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

        public async Task<bool> PullImage(string repoName, string tag)
        {
            await _client.Images.CreateImageAsync(new ImagesCreateParameters()
            {
                FromImage = repoName,
                Tag = tag
            }, null, new ContainerProgress(), default);

            return true;
        }

        public async Task<Stream> BuildImageFromDockerFile(Stream dockerFile)
        {
            var result = await _client.Images.BuildImageFromDockerfileAsync(dockerFile, new ImageBuildParameters
            {

            });

            return result;
        }

        public async Task<Stream> SaveImage(string imageName)
        {
            var result = await _client.Images.SaveImageAsync(imageName);

            return result;
        }

        public async Task<bool> LoadImage(Stream imageStream)
        {
            await _client.Images.LoadImageAsync(new ImageLoadParameters { }, imageStream, new ContainerProgress());

            return true;
        }

        public async Task<ImageInspectResponse> InspectImage(string imageName)
        {
            var result = await _client.Images.InspectImageAsync(imageName);

            return result;
        }

        public async Task<IList<ImageHistoryResponse>> GetImageHistory(string imageName)
        {
            var result = await _client.Images.GetImageHistoryAsync(imageName);

            return result;
        }

        public async Task<bool> TagImage(string imageName, string tagName)
        {
            await _client.Images.TagImageAsync(imageName, new ImageTagParameters
            {
                Tag = tagName
            });

            return true;
        }
    }
}
