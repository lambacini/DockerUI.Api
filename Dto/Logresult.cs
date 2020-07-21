using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DockerUI.Api.Dto
{
    public  class LogResult
    {
        public List<string> Logs = new List<string>();
    }
}
