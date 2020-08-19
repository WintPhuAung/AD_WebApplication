using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Team7_StationeryStore.Models;

namespace Team7_StationeryStore.Database
{
    public class DbSeeder
    {

        public DbSeeder(StationeryContext dbcontext)
        {
           
            Employee employee1 = new Employee();
            
            employee1.Name = "ava";
            employee1.Email = "ava@gmail.com";
            employee1.Gender = "Male";
            employee1.DateReg = "2/2/2020";
            employee1.SelectedDays = "day1";
            employee1.AdditionalRequest ="To provide rooms";
            
            dbcontext.Add(employee1);
            dbcontext.SaveChanges();




        }
    }
}
