﻿using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DockerUI.Api.Services
{
    public class DockerService
    {
        private ILogger<DockerService> _logger;
        private DockerClient _client;

        public DockerService(ILogger<DockerService> logger)
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

        public async Task<SystemInfoResponse> GetSystemInfo(string containerid)
        {
            var result = await _client.System.GetSystemInfoAsync();

            return result;
        }

        public async Task<VersionResponse> GetVersion(string containerid)
        {
            var result = await _client.System.GetVersionAsync();

            return result;
        }

    }
}
