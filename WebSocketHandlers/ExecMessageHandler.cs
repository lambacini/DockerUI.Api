using DockerUI.Api.Dto;
using DockerUI.Api.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace DockerUI.Api.WebSocketHandlers
{
    public class ExecMessageHandler : WebSocketHandler
    {
        public ExecMessageHandler(ConnectionManager webSocketConnectionManager)
            : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{socketId} is now connected");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);

            ExecContainerResponse responseMessage;
            try
            {
                var messageStr = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var messageJson = JsonConvert.DeserializeObject<ExecContainerRequest>(messageStr);

                ContainerExecSession session = new ContainerExecSession(WebSocketConnectionManager.GetSocketById(socketId), messageJson);
                await session.Exec();
            }
            catch (Exception ex)
            {
                responseMessage = new ExecContainerResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
                var responseStr = JsonConvert.SerializeObject(responseMessage);

                await SendMessageAsync(socketId, responseStr);

            }

        }
    }
}
