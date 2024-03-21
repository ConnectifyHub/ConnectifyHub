using Server.Data.Entities;
using System.IO;
using System.Text.RegularExpressions;

namespace Server
{
    public static class ValidationHelper
    {
        public static bool IsValidEmailAndPassword(string[] parts)
        {
            User user = DatabaseUtils.GetUserByEmail(parts[0]);
            return user != null && user.Password == parts[1];
        }
        public static bool IsValidEmail(string[] parts)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(parts[0]);
                return addr.Address == parts[0];
            } catch
            {
                return false;
            }
        }

        public static bool IsStrongPassword(string[] parts)
        {
            int minLength = 8;
            bool hasUpperCase = parts[0].Any(char.IsUpper);
            bool hasLowerCase = parts[0].Any(char.IsLower);
            bool hasDigit = parts[0].Any(char.IsDigit);

            return parts[0].Length >= minLength && hasUpperCase && hasLowerCase && hasDigit;
        }

        public static bool IsValidFirstName(string[] parts)
        {
            return Regex.IsMatch(parts[0], @"^[A-Za-zА-Яа-я]+$");
        }

        public static bool IsValidLastName(string[] parts)
        {
            return Regex.IsMatch(parts[0], @"^[A-Za-zА-Яа-я]+$");
        }

        public static bool IsValidPhoneNumber(string[] parts)
        {
            return Regex.IsMatch(parts[0], @"^(\+\d{12}|\d{9})$");
        }
    }
}
