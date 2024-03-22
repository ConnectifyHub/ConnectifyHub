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
        public static User ParseUserFromString(string[] parts)
        {

            User newUser = new User();

            if (parts.Length > 0)
                newUser.Email = parts[0];
            if (parts.Length > 1)
                newUser.Password = parts[1];
            if (parts.Length > 2)
                newUser.Name = parts[2];
            if (parts.Length > 3)
                newUser.Surname = parts[3];

            return newUser;
        }
    }
}
