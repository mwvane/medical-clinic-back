using medical_clinic.Models;
using System.Text.RegularExpressions;

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

        public static  List<string> Validateuser(User user)
        {
            List<string> errors = new List<string>();
            if (user.Firstname.Length < 5)
            {
                errors.Add("სახელი უნდა შედგებოდეს არა ნაკლებ 5 სიმბოლოსგან");
            }
            if (user.Password.Length < 8)
            {
                errors.Add("პაროლი უნდა შედგებოდეს არა ნაკლებ 8 სიმბოლოსგან");
            }
            var capitalLetters = Regex.IsMatch(user.Password, "[A-Z]");
            if (!capitalLetters)
            {
                errors.Add("პაროლი უნდა შეიცავდეს მინიმუმ ერთ კაპიტალურ სიმბოლოს");
            }

            var isDigit = Regex.IsMatch(user.Password, "[0-9]");
            if (!capitalLetters)
            {
                errors.Add("პაროლი უნდა შეიცავდეს მინიმუმ ერთ ციფრს");
            }

            if (IsEmailValid(user.Email))
            {
                errors.Add("მეილის ფორმატია");

            }
            return errors;
        }

        public static  bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, "^[a-zA-Z0-9_.+-]+@[email]+\\.[a-zA-Z0-9-.]+$", RegexOptions.IgnoreCase) == true;
        }
    }
}
