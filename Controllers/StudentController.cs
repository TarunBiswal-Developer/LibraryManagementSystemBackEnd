using BackEnd.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class StudentController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public StudentController ( UserManager<IdentityUser> userManager, IConfiguration configuration, ApplicationDbContext context )
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;

        }



    }
}
