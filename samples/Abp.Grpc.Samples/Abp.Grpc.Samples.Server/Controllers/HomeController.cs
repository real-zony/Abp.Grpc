using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Abp.Grpc.Samples.Server.Controllers
{
    public class HealthController : AbpController
    {
        public HealthController()
        {
            
        }
        
        // GET
        public IActionResult Check()
        {
            return Ok("OJBK");
        }
    }
}