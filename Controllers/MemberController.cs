using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class MemberController : ControllerBase
    {
        public IActionResult Index ()
        {
            return Ok();
        }
    }
}
