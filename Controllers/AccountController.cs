using BackEnd.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenService _tokenService;

        public AccountController ( UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, TokenService tokenService )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }


        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken ( [FromBody] TokenRequest request )
        {
            try
            {
                if (ModelState.IsValid && request.GrantType == "password")
                {
                    var user = await _userManager.FindByNameAsync(request.Username);
                    if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var token = _tokenService.GenerateJwtToken(user, roles);
                        var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        var generatedTime = DateTime.Now;
                        var expiryTime = generatedTime.AddHours(1);
                        return Ok(new { token, generatedTime, expiryTime });
                    }
                }
                else
                {
                    return BadRequest("grant_tpe must be password");
                }
            }
            catch (Exception)
            {
                throw;
            }
            
            return BadRequest("Incorrect Username and Password");
        }


    }
}
