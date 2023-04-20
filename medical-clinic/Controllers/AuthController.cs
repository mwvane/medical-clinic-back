using MailKit.Net.Smtp;
using medical_clinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace medical_clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _context;
        public AuthController(MyDbContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public Result Register([FromBody] User user)
        {

            if (user != null)
            {
                List<string> errors = validationHelper.Validateuser(user);
                user.CreatedAt = DateTime.Now;
                bool usernameAlreadyExists = false;

                usernameAlreadyExists = _context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null;

                if (!usernameAlreadyExists)
                {

                    if (errors.Count == 0)
                    {
                        _context.Add(user);
                        _context.SaveChanges();

                        if (user.Role == "doctor")
                        {
                            _context.Doctors.Add(new Doctor { UserID = user.Id, Rating = 0, Views = 0 });
                            _context.SaveChanges();
                        }
                        return new Result() { Res = user };
                    }
                    return new Result() { Errors = errors };
                }
                return new Result() { Errors = new List<string>() { "მომხმარებლის სახელი უკვე არსებობს!" } };

            }
            return new Result() { Errors = new List<string>() { "მონაცემები არასწორია" } };
        }


        [HttpPost("login")]
        public Result login([FromBody] Dictionary<string, string> payload)
        {
            string username = payload["username"];
            string password = payload["password"];
            if (username == null || password == null)
            {
                return new Result() { Errors = new List<string> { "მომხმარებლის სახელი და პაროლი აუცილებელია!" } };
            }
            else
            {
                var user = _context.Users.Where(user => user.Email == username && user.Password == password).FirstOrDefault();

                if (user != null)
                {
                    user.Token = CreateJwt(user);
                    return new Result()
                    {
                        Res = new JwtAuthResponse
                        {
                            Token = user.Token,
                        }
                    };
                }
                return new Result() { Errors = new List<string> { "მომხმარებლის სახელი ან პაროლი არასწორია!" } };
            }
        }

        [HttpPost("sendMail")]
        public void SendMail([FromBody] Mail mail)
        {
            Console.WriteLine(mail.EmailTo[0]);
            var code = Guid.NewGuid().ToString("N").Substring(0, 8);
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Constants.EMAIL_FROM));
            email.To.Add(MailboxAddress.Parse(Constants.EMAIL_FROM));
            email.Subject = "test subject";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "Your code is : " + code };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(Constants.EMAIL_FROM, Constants.MAIL_PASSWORD);
            smtp.Send(email);
            smtp.Disconnect(true);

            var emailConfirm = _context.EmailConfirm.Where(item => item.Email == mail.EmailTo[0]).FirstOrDefault();
            if (emailConfirm != null)
            {
                emailConfirm.ValidDate = DateTime.Now.AddMinutes(30);
                emailConfirm.Code = code;
                _context.EmailConfirm.Update(emailConfirm);
            }
            else
            {
                _context.EmailConfirm.Add(new EmailConfirm { Code = code, Email = mail.EmailTo[0], ValidDate = DateTime.Now.AddMinutes(30) });
            }
            _context.SaveChanges();

        }

        [HttpPost("emailConfirm")]
        public Result EmailConfirm([FromBody] Dictionary<string, string> payload)
        {
            var payloadEmail = payload["email"];
            var payloadCode = payload["code"];
            var result = _context.EmailConfirm.Where(item => item.Email == payloadEmail && item.Code == payloadCode).FirstOrDefault();
            if (result != null)
            {
                if (result.ValidDate < DateTime.Now)
                {
                    return new Result() { Errors = new List<string>() { "კოდი ვადაგასულია" } };
                }
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "კოდი არასწორია" } };

        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Constants.JWT_SECURITY_KEY);
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim("lastname",$"{user.Lastname}"),
                new Claim("firstname",$"{user.Firstname}"),
                new Claim("ID",$"{user.IdentityNumber}"),
                new Claim(ClaimTypes.Email,$"{user.Email}"),
            }); ;
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(Constants.JWT_TOKEN_VALIDITY_MINS),
                SigningCredentials = credentials,
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
