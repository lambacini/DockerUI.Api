using Docker.DotNet;
using DockerUI.Api.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DockerUI.Api.Helpers
{


    public class ContainerExecSession
    {
        private DockerClient _client;
        private ExecContainerRequest _initialRequest;
        private WebSocket _webSocket;

        public ContainerExecSession(WebSocket socket, ExecContainerRequest initialRequest)
        {
            _webSocket = socket;
            _initialRequest = initialRequest;
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

        public async Task Exec()
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            

        }
        public async Task StartStream()
        {
            var execCreateResp = await _client.Exec.ExecCreateContainerAsync(_initialRequest.ContainerID, new Docker.DotNet.Models.ContainerExecCreateParameters
            {
                AttachStderr = true,
                AttachStdin = true,
                AttachStdout = true,
                Cmd = new string[] { _initialRequest.Command },
                Env = new string[] { "PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin" },
                Detach = false,
                Tty = true,
                Privileged = true
            });

            using (var stream = await _client.Exec.StartAndAttachContainerExecAsync(execCreateResp.ID, false))
            {
                var execInspectResp = await _client.Exec.InspectContainerExecAsync(execCreateResp.ID);
                var pid = execInspectResp.Pid;


                var tRead = Task.Run(async () =>
                {
                    var dockerBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(81920);
                    try
                    {
                        while (true)
                        {
                            Array.Clear(dockerBuffer, 0, dockerBuffer.Length);
                            var dockerReadResult = await stream.ReadOutputAsync(dockerBuffer, 0, dockerBuffer.Length, default);

                            if (dockerReadResult.EOF)
                                break;

                            if (dockerReadResult.Count > 0)
                            {
                                bool endOfMessage = true;

                                var execResponse = Encoding.UTF8.GetString(dockerBuffer);
                                execResponse =  execResponse.Replace("\0", "");
                                var response = new ExecContainerResponse
                                {
                                    ContainerID = _initialRequest.ContainerID,
                                    Success = true,
                                    Message = execResponse
                                };

                                var responseText = JsonConvert.SerializeObject(response);

                                await _webSocket.SendAsync(
                                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(responseText),
                                                                    offset: 0,
                                                                    count: responseText.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);

                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write("Failure during Read from Docker Exec to WebSocket " + ex.Message);
                    }
                    System.Buffers.ArrayPool<byte>.Shared.Return(dockerBuffer);
                });


                // Write WS to Docker                             
                var tWrite = Task.Run(async () =>
                {
                    WebSocketReceiveResult wsReadResult = null;
                    
                    var wsBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(10);
                    try
                    {
                        while (true)
                        {
                            Array.Clear(wsBuffer, 0, wsBuffer.Length);
                            wsReadResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(wsBuffer), CancellationToken.None);

                            await stream.WriteAsync(wsBuffer, 0, wsBuffer.Length, default(CancellationToken));



                            if (wsReadResult.CloseStatus.HasValue)
                            {
                                var killSequence = Encoding.ASCII.GetBytes($"Stop-Process -Id {pid}{Environment.NewLine}");
                                await stream.WriteAsync(killSequence, 0, killSequence.Length,
                                    default(CancellationToken));
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    System.Buffers.ArrayPool<byte>.Shared.Return(wsBuffer);
                });

                await tRead;
                await tWrite;

            }
        }
    }
}
