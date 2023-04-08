using medical_clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace medical_clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly MyDbContext _context;

        public BookController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("addBook")]
        public Result AddBook([FromBody] Book book)
        {
            var res = validationHelper.ValidateBook(book, _context);
            if (res.Errors.Count > 0)
            {
                return res;
            }
            else
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return new Result() { Res = book };
            }
        }

        [HttpGet("getBookedDays")]
        public Result GetBookedDays(int id = 6)
        {
            Console.WriteLine(id);
            var bookedDays = _context.Books.Where(item => item.DoctorId == id).ToList();
            if(bookedDays == null|| bookedDays.Count == 0)
            {
                return new Result() { Errors = new List<string>() { "ჩანაწერი ვერ მოიძებნა" } };
            }
            return new Result() { Res = bookedDays};
        }

    }
}
