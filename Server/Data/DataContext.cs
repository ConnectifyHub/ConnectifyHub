using Microsoft.EntityFrameworkCore;
using System.IO;
using Server.Data.Entities;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Server.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText("appconfig.json"));
            optionsBuilder.UseMySql(config["databases"]["planetScale"]["connectionString"].ToString(), new MySqlServerVersion(new Version(8, 0, 34)));
        }
    }
}
