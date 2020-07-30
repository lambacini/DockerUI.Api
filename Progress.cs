using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerUI.Api
{
    public class ContainerProgress : IProgress<JSONMessage>
    {
        public ContainerProgress()
        {

        }

        public void Report(JSONMessage value)
        {
            Console.WriteLine(value.ToString());
        }
    }
}
