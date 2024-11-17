using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace SyncNode.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Sync(SyncEntity entity)
        {
            return Ok();
        }
    }
}
