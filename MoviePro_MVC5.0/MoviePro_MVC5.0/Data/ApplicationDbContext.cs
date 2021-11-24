using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviePro_MVC5._0.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviePro_MVC5._0.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Collection> Collection { get; set; }
    }
}
