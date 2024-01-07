using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText("appconfig.json"));
            optionsBuilder.UseMySql(config["databases"]["planetScale"]["connectionString"].ToString(), new MySqlServerVersion(new Version(8, 0, 34)));
        }
    }
}
