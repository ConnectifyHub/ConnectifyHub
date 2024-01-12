using Server.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class DataStrParser
    {
        public static User ParseUserFromString(string userData)
        {
            string[] parts = userData.Split('|');

            // Assuming your User class has these properties
            User newUser = new User
            {
                Email = parts[0],
                Password = parts[1],
                Name = parts[2],
                Surname = parts[3],
                Phone = parts[4]
            };

            return newUser;
        }
    }
}
