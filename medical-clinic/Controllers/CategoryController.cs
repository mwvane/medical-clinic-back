using medical_clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace medical_clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyDbContext _context;
        public CategoryController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("getCategories")]
        public Result GetCategories()
        {
            var categories = _context.Categories.ToList();
            if (categories != null)
            {
                return new Result() { Res = categories };
            }
            return new Result() { Errors = new List<string>() { "no categories found" } };
        }

        [Authorize]
        [HttpPost("add")]
        public Result Add([FromBody] Category category)
        {
            if (category.Name.Trim().Length > 0)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return new Result() { Res = category };
            }
            return new Result() { Errors = new List<string>() { "კატეგორიის სახელი აუცილებებლია" } };

        }

        [Authorize]
        [HttpPost("delete")]
        public Result Delete([FromBody] int id)
        {
            var category = _context.Categories.Where(item => item.Id == id).FirstOrDefault();
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "კატეგორია ვერ მოიძებნა" } };

        }

        [Authorize]
        [HttpPost("update")]
        public Result Update([FromBody] Category updatedCategory)
        {
            var category = _context.Categories.Where(item => item.Id == updatedCategory.Id).FirstOrDefault();
            if (category != null)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "კატეგორია ვერ მოიძებნა" } };

        }
    }
}
