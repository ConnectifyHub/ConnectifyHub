using Server.Data.Entities;
using System.IO;
using System.Text.RegularExpressions;

namespace Server
{
    public static class ValidationHelper
    {
        public static bool IsValidEmailAndPassword(string[] parts)
        {
            User user = DatabaseUtils.GetUserByEmail(parts[1]);
            return user != null && user.Password == parts[2];
        }
        public static bool IsValidEmail(string[] parts)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(parts[1]);
                return addr.Address == parts[1];
            } catch
            {
                return false;
            }
        }

        public static bool IsStrongPassword(string[] parts)
        {
            int minLength = 8;
            bool hasUpperCase = parts[1].Any(char.IsUpper);
            bool hasLowerCase = parts[1].Any(char.IsLower);
            bool hasDigit = parts[1].Any(char.IsDigit);

            return parts[1].Length >= minLength && hasUpperCase && hasLowerCase && hasDigit;
        }

        public static bool IsValidFirstName(string[] parts)
        {
            return Regex.IsMatch(parts[1], @"^[A-Za-zА-Яа-я]+$");
        }

        public static bool IsValidLastName(string[] parts)
        {
            return Regex.IsMatch(parts[1], @"^[A-Za-zА-Яа-я]+$");
        }

        public static bool IsValidPhoneNumber(string[] parts)
        {
            return Regex.IsMatch(parts[1], @"^(\+\d{12}|\d{9})$");
        }
    }
}
