﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LAB1.Models;

namespace LAB1.Data
{
    public class LAB1Context : DbContext
    {
        public LAB1Context(DbContextOptions<LAB1Context> options)
            : base(options)
        {
        }

        public DbSet<Manga> Manga { get; set; } = default!;

        // Зміна назви Author на Authors
        public DbSet<Author> Authors { get; set; } = default!;

        // Зміна назви Tag на Tags
        public DbSet<Tag> Tags { get; set; } = default!;
    }
}
