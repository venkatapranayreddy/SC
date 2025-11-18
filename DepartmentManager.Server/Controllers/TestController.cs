using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DepartmentManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string GetClaimValue(string key)
        {
            if (HttpContext.User == null || HttpContext.User.Claims == null || HttpContext.User.Claims.Count() == 0)
                return string.Empty;
            return HttpContext.User.Claims.Where(x => x.Type == key).FirstOrDefault().Value;
        }
        public TestController()
        {

        }

        [Route("Test")]
        [HttpGet]
        public async Task<IActionResult> GetTest()
        {
            return Ok(new
            {
                a = "TEST RESULT"
            });
        }
        
    }
}
