using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kundregister.Controllers
{
    [Route("log")]
    public class LogController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var getDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var log = System.IO.File.ReadAllLines($"{getDirectory}/log/nlog-own.log");
            return log;
        }
    }
}
