using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Application.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //get the connection string from the appconfig.json file
            var config = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText("appconfig.json"));
            optionsBuilder.UseMySql(config["databases"]["planetScale"]["connectionString"].ToString(), new MySqlServerVersion(new Version(8, 0, 34)));
        }
    }
}
