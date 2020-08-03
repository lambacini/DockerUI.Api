using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerUI.Api.Dto
{
    public class ExecContainerRequest
    {
        public bool Connected { get; set; }
        public string ExecID { get; set; }
        public string ContainerID { get; set; }
        public string Command { get; set; }
    }
}
