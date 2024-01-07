using System.Text.RegularExpressions;

namespace Server
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsStrongPassword(string password)
        {
            int minLength = 8;
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return password.Length >= minLength && hasUpperCase && hasLowerCase && hasDigit;
        }

        public static bool IsValidFirstName(string firstName)
        {
            return Regex.IsMatch(firstName, @"^[A-Za-zА-Яа-я]+$");
        }

        public static bool IsValidLastName(string lastName)
        {
            return Regex.IsMatch(lastName, @"^[A-Za-zА-Яа-я]+$");
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^(\+\d{12}|\d{9})$");
        }
    }
}
