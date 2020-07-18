using DockerUI.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerUI.Api.Data
{
    public class DockerUIContext : DbContext
    {
        public DockerUIContext(DbContextOptions options)
          : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DockerUI.db");
        }

        public DbSet<AppUser> Users { get; set; }
    }
}
