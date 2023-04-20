﻿using medical_clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [Authorize(Roles = "admin")]
        [HttpGet("getUsersByRole")]
        public Result GetUsersByRole(string role = "doctor")
        {

            var users = _context.Users.Where(item => item.Role == role).ToList();
            if (users != null)
            {
                return new Result() { Res = users };
            }
            return new Result() { Errors = new List<string>() { "მომხმარებელი ვერ მოიძებმა" } };
        }

        [Authorize(Roles = "admin")]
        [HttpPost("deleteUser")]
        public Result DeleteUser([FromBody] int userId)
        {
            var user = _context.Users.Where(item => item.Id == userId).FirstOrDefault();
            if (user != null)
            {
                if(user.Role == "doctor")
                {
                    var doctor = _context.Doctors.Where(item => item.UserID == user.Id).FirstOrDefault();
                    if(doctor != null)
                    {
                        _context.Doctors.Remove(doctor);
                    }
                }
                _context.Users.Remove(user);
                //_context.SaveChanges();
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "მსგავსი მომხმარებელი ვერ მოიძებნა!" } };
        }

        [HttpPost("editUser")]
        public Result EditUser([FromBody] User user)
        {
            List<string> errors = validationHelper.Validateuser(user);
            if (errors.Count == 0)
            {
                var selectedUser = _context.Users.Where(item => item.Id == user.Id).FirstOrDefault();

                if (selectedUser != null)
                {
                    selectedUser.IdentityNumber = user.IdentityNumber;
                    selectedUser.Role = user.Role;
                    selectedUser.Firstname = user.Firstname;
                    selectedUser.Lastname = user.Lastname;
                    selectedUser.ImageUrl = user.ImageUrl;
                    selectedUser.Password = user.Password;
                    _context.Users.Update(selectedUser);
                    _context.SaveChanges();
                    return new Result() { Res = true };

                }
                return new Result() { Errors = new List<string>() { "მონაცემები არასწორია" } };

            }
            else
            {
                return new Result() { Errors = errors };
            }
        }
    }
}

