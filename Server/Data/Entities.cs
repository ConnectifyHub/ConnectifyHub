using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string? ChatId { get; set; }
        public int? AuthorId { get; set; }
        public string? Text { get; set; }
        public DateTimeOffset Sended { get; set; } = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

    }

    public class ChatUser
    {
        public int Id { get; set; }
        public int? ChatId { get; set; }
        public int? UserId { get; set; }
    }

    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int AuthorId { get; set; } = 0;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string Password { get; set; } = ""; 
        public string Email { get; set; } = "";
        public int ChatIdSelected { get; set; } = 0;
        public string PublicIdentityKey { get; set; } = ""; 

    }
}
