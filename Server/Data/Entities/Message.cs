﻿using System;
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
}
