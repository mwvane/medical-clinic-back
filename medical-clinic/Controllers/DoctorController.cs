using medical_clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            Claim? claimId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
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
                               ImageUrl = user.ImageUrl,
                               Category = _context.Categories.Where(item => item.Id == doctor.CategoryId).FirstOrDefault(),
                               Pin = _context.Pin.Where(item => item.DoctorId == user.Id && item.UserId == Convert.ToInt32((claimId != null ? claimId.Value : 0))).FirstOrDefault(),

                           }).ToList();

            if (id == 0)
            {
                var date = new DateTime();
                return new Result() { Res = doctors.OrderByDescending(item => item.Pin != null ? item.Pin.PinDate : date) };
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

        [Authorize]
        [HttpPost("pin")]
        public Result Pin([FromBody] Pin pin)
        {
            Result error = new Result() { Errors = new List<string>() { "მსგავსი მომხმარებელი ვერ მოიძებნა" } };
            if (pin.UserId > 0 && pin.DoctorId > 0)
            {
                var existedPin = _context.Pin.Where(item => item.UserId == pin.UserId && item.DoctorId == pin.DoctorId).FirstOrDefault();
                if (existedPin != null)
                {
                    existedPin.IsPinned = pin.IsPinned;
                    if (pin.IsPinned)
                    {
                        existedPin.PinDate = DateTime.Now;
                        _context.Pin.Update(existedPin);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Pin.Remove(existedPin);
                        _context.SaveChanges();
                    }

                    Claim? claimId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
                    var pinCount = _context.Pin.Where(item => item.UserId == Convert.ToInt32(claimId.Value)).ToList().Count;
                    return new Result() { Res = pinCount == 0 ? 0 : pinCount };
                }
                else
                {
                    _context.Pin.Add(pin);
                    _context.SaveChanges();
                    return new Result() { Res = 0 };
                }
            }
            else
            {
                return error;
            }
        }
    }
}