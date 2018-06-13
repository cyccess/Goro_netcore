using Goro.Check.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Goro.Check.Data
{
    public class CheckDbContext : DbContext
    {
        public CheckDbContext(DbContextOptions<CheckDbContext> options) : base(options)
        {

        }

        public DbSet<UserInfo> UserInfo { get; set; }

    }
}
