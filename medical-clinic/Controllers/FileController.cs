using medical_clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace medical_clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly MyDbContext _context;
        public FileController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("uploadImage"), DisableRequestSizeLimit]
        public string? UploadProductImage([FromForm] UploadFile file)
        {
            try
            {
                var foldername = Path.Combine("Resources", "Images");
                var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername);
                string imageUrl = "";
                if (file.File.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.File.ContentDisposition).FileName.Trim().ToString();
                    var fullPath = Path.Combine(pathTosave, fileName);
                    var dbPath = Path.Combine(foldername, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.File.CopyTo(stream);
                        imageUrl = fullPath;
                    }
                    var user = _context.Users.FirstOrDefault(item => item.Id == file.UserId);
                    if (user != null)
                    {
                        user.ImageUrl = dbPath;
                        _context.Users.Update(user);
                        _context.SaveChanges();
                    }

                }
                _context.SaveChanges();

                return imageUrl;
            }
            catch
            {
                return null;
            }
        }

        //    [Authorize]
        //    [HttpPost("uploadDocument"), DisableRequestSizeLimit]

        //    public List<string> UploadProductDocument([FromForm] UploadFile files)
        //    {
        //        try
        //        {
        //            var foldername = Path.Combine("Resources", "Documents");
        //            var pathTosave = Path.Combine(Directory.GetCurrentDirectory(), foldername);
        //            List<string> documentUrls = new List<string>();
        //            foreach (var item in files.files)
        //            {
        //                if (item.Length > 0)
        //                {
        //                    var fileName = ContentDispositionHeaderValue.Parse(item.ContentDisposition).FileName.Trim().ToString();
        //                    var fullPath = Path.Combine(pathTosave, fileName);
        //                    var dbPath = Path.Combine(foldername, fileName);
        //                    using (var stream = new FileStream(fullPath, FileMode.Create))
        //                    {
        //                        item.CopyTo(stream);
        //                        documentUrls.Add(fullPath);
        //                    }
        //                    _context.ProductDocuments.Add(new ProductDocument() { ProductId = files.productId, DocumentUrl = dbPath });

        //                }
        //            }
        //            _context.SaveChanges();

        //            return documentUrls;
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }

    }
}
