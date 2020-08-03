using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerUI.Api.Dto
{
    public class ExecContainerResponse
    {
        public string ExecID { get; set; }
        public bool Success { get; set; }
        public string ContainerID { get; set; }
        public string Message { get; set; }
    }
}
