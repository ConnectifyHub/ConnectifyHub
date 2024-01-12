using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Data.Entities;
using Server.Data;

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

        public static void UpdateUser(int userId, User updatedUser)
        {
            using (var context = GetContext())
            {
                var userToUpdate = context.Users.Find(userId);

                if (userToUpdate != null)
                {
                    userToUpdate.Name = updatedUser.Name;
                    userToUpdate.Surname = updatedUser.Surname;
                    userToUpdate.Phone = updatedUser.Phone;
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
