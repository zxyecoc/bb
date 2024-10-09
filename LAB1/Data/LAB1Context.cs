using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LAB1.Models;

namespace LAB1.Data
{
    public class LAB1Context : DbContext
    {
        public LAB1Context (DbContextOptions<LAB1Context> options)
            : base(options)
        {
        }

        public DbSet<LAB1.Models.Manga> Manga { get; set; } = default!;
    }
}
