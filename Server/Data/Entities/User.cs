﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Entities
{
    public class User
    {
        public bool IsConfirmed { get; set; } = false;
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;

    }
}
