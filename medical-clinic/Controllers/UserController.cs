using medical_clinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace medical_clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("getUser")]
        public Result GetUser(int id = 0)
        {
            if (id > 0)
            {
                var user = _context.Users.Where(item => item.Id == id).FirstOrDefault();
                if (user != null)
                {
                    return new Result() { Res = user };
                }
                return new Result() { Errors = new List<string>() { "მსგავსი მომხმარებელი ვერ მოიძებმა" } };
            }

            else
            {
                return new Result() { Res = _context.Users.ToList() };
            }
        }
    }
}
