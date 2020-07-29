using DockerUI.Api.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerUI.Api.Hubs
{
    public class ContainerHub : Hub
    {
        ContainerService _service;
        public ContainerHub(ContainerService containerService)
        {
            _service = containerService;
        }

        public async Task SubscribeStats(string containerid)
        {
            var containers = await _service.GetContainerLists(200);
            var container = containers.FirstOrDefault(p => p.ID == containerid);

            using (var stream = await _service.GetContainerStats(containerid))
            {
                UTF8Encoding encoding = new UTF8Encoding(false);
                using (var reader = new StreamReader(stream, encoding))
                {
                    string nextLine;

                    while ((nextLine = await reader.ReadLineAsync()) != null)
                    {
                        await Clients.Caller.SendAsync("ContainerStats", nextLine);
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
        }
    }
}
