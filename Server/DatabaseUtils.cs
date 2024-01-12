using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Data.Entities;
using Server.Data;
using Newtonsoft.Json;

namespace Server
{
    internal class DatabaseUtils
    {
        private static DataContext GetContext()
        {
            return new DataContext();
        }

        public static void AddUser(User newUser)
        {
            using (var context = GetContext())
            {
                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }

        public static List<User> GetUsers()
        {
            using (var context = GetContext())
            {
                return context.Users.ToList();
            }
        }

        public static User GetUserById(int userId)
        {
            using (var context = GetContext())
            {
                return context.Users.Find(userId);
            }
        }

        public static User GetUserByEmail(string email)
        {
            using (var context = GetContext())
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        public static User GetUserByKey(string key)
        {
            using (var context = GetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.PublicIdentityKey == key);
                if (user != null)
                {
                    GeneratePublicKey(user.Email);
                }
                return user;
            }
        }

        public static string GetUserByKeyStringified(string key)
        {
            using (var context = GetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.PublicIdentityKey == key);
                if (user != null)
                {
                    return $"{user.Email}|{user.Name}|{user.Surname}";
                }
                return null;
            }
        }

        static string GenerateRandomKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            string randomKey = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomKey;
        }

        public static string GeneratePublicKey(string email)
        {
            string Pkey;
            using (var context = GetContext())
            {
                Pkey = GenerateRandomKey(16);
                while ((context.Users.FirstOrDefault(u => u.PublicIdentityKey == Pkey) != null)) {
                    Pkey = GenerateRandomKey(16);
                }
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                user.PublicIdentityKey = Pkey;
                context.SaveChanges();
            }
            return Pkey;
        }

        public static string GetPublicKey(string email, string password)
        {
            using (var context = GetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
                return user.PublicIdentityKey;
            }
        }

        public static void UpdateUser(int userId, User updatedUser)
        {
            using (var context = GetContext())
            {
                var userToUpdate = context.Users.Find(userId);

                if (userToUpdate != null)
                {
                    userToUpdate.Name = updatedUser.Name;
                    userToUpdate.Surname = updatedUser.Surname;
                    userToUpdate.Password = updatedUser.Password;
                    userToUpdate.Email = updatedUser.Email;

                    context.SaveChanges();
                }
            }
        }

        public static void DeleteUser(int userId)
        {
            using (var context = GetContext())
            {
                var userToDelete = context.Users.Find(userId);

                if (userToDelete != null)
                {
                    context.Users.Remove(userToDelete);
                    context.SaveChanges();
                }
            }
        }
    }
}
