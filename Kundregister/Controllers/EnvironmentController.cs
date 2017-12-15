using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Kundregister.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kundregister.Controllers
{
    [Route("api/environment")]
    public class EnvironmentController : Controller
    {
        private readonly IHostingEnvironment env;
        private readonly MailConfiguration mailConfiguration;

        public EnvironmentController(IHostingEnvironment env, MailConfiguration mailConfiguration)
        {
            this.env = env;
            this.mailConfiguration = mailConfiguration;
        }

        public IActionResult Info()
        {
            return Ok(new object[]
            {
                env.IsDevelopment(),
                env.IsProduction(),
                env.ContentRootPath,
                env.ApplicationName,
                env.EnvironmentName,
                env.WebRootPath,
                mailConfiguration
            });
        }
    }
}
