using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team7_StationeryStore.Models;


namespace Team7_StationeryStore.Database
{
    public class StationeryContext : DbContext
    {
        public StationeryContext(DbContextOptions<StationeryContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder model)
        {
            
        }

        public DbSet<Employee> employees { get; set; }

    }
}
