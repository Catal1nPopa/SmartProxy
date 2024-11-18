using Common.Models;
using Microsoft.AspNetCore.Mvc;
using SyncNode.Services;

namespace SyncNode.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SyncController(SyncWorkJobService syncWorkJobService) : ControllerBase
    {
        private readonly SyncWorkJobService _jobService = syncWorkJobService;

        [HttpPost]
        public async Task<IActionResult> Sync(SyncEntity entity)
        {
            _jobService.AddItem(entity);
            return Ok();
        }
    }
}
