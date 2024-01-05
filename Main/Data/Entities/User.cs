using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }       = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Password { get; set; }    = null!;
        public string Email { get; set; }       = null!;

    }
}
