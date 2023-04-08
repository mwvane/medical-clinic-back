using medical_clinic.Models;

namespace medical_clinic
{
    public static class validationHelper
    {
        public static Result ValidateBook(Book book, MyDbContext context)
        {
            Result result = new Result() { Errors = new List<string>() };
            if (book == null)
            {
                result.Errors.Add("ჯავშანი ცარიელია");
            }
            if (book.Date < DateTime.Now)
            {
                result.Errors.Add("მითითებულ დღეს დაჯავშნა შეუძლებელია!");
            }
            var isAlreadyBooked = context.Books.Where(item => item.DoctorId == book.DoctorId && item.Date == book.Date).FirstOrDefault() != null;
            if (isAlreadyBooked)
            {
                result.Errors.Add("მითითებული დრო დაკავებულია!");
            }
            return result;
        }
    }
}
