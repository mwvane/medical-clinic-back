using medical_clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace medical_clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly MyDbContext _context;
        public DoctorController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("getDoctors")]
        public Result GetDoctors(int id = 0)
        {
            var doctors = (from user in _context.Users
                           join doctor in _context.Doctors
                           on user.Id equals doctor.UserID
                           select new
                           {
                               Id = user.Id,
                               Firstname = user.Firstname,
                               Lastname = user.Lastname,
                               IdentityNumber = user.IdentityNumber,
                               Email = user.Email,
                               Role = user.Role,
                               Rating = doctor.Rating,
                               Views = doctor.Views,
                               IsPinned = doctor.IsPinned,
                               ImageUrl = user.ImageUrl,
                               Category = _context.Categories.Where(item => item.Id == doctor.CategoryId).FirstOrDefault(),

                           }).ToList();
            if (id == 0)
            {
                return new Result() { Res = doctors };
            }
            else
            {
                var doctor = doctors.Where(item => item.Id == id).FirstOrDefault();
                if (doctor != null)
                {
                    return new Result() { Res = doctor };
                }
                return new Result() { Errors = new List<string>() { "ექიმი ვერ მოიძებნა" } };
            }
        }

        [Authorize]
        [HttpPost("increaseDoctorViews")]
        public Result IncreaseDoctorViews([FromBody] int docotorId)
        {
            Claim? claimId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
            if (claimId != null)
            {
                var loggeduserId = Convert.ToInt32(claimId.Value);

                var doctorView = _context.DoctorViews.Where(item => item.UserId == loggeduserId && item.DoctorId == docotorId).FirstOrDefault();
                if (doctorView == null)
                {
                    var doctor = _context.Doctors.Where(item => item.UserID == docotorId).FirstOrDefault();
                    if (doctor != null)
                    {
                        doctor.Views++;
                        _context.Doctors.Update(doctor);
                        _context.DoctorViews.Add(new DoctorViews() { DoctorId = docotorId, UserId = loggeduserId });
                        _context.SaveChanges();
                        return new Result() { Res = doctor.Views };
                    }

                }
                return new Result() { Errors = new List<string>() { "ექიმი ვერ მოიძებნა" } };

            }
            return new Result() { Errors = new List<string>() { "ავტორიზაცია საჭიროა" } };

        }
    }
}
