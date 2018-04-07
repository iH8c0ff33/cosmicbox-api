using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CosmicBox.Controllers {
    [Route("api/[controller]")]
    public class UserController : Controller {
        [HttpGet("authorize"), Authorize("test")]
        public IActionResult Authorize([FromQuery] string redirect) {
            if (string.IsNullOrWhiteSpace(redirect)) {
                return BadRequest();
            }

            return new NoContentResult();
        }
    }
}