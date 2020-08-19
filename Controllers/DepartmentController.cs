using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Team7_StationeryStore.Models;
using Team7_StationeryStore.Database;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Team7_StationeryStore.Controllers
{
    public class DepartmentController : Controller
    {
        protected StationeryContext dbcontext;
        


        public DepartmentController(StationeryContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

     

        public IActionResult viewUsers()
        {
            string userid = HttpContext.Session.GetString("userId");
            List<Employee> users = dbcontext.employees.ToList();           
            ViewData["users"] = users;
            return View();
        }

        public IActionResult addUser()
        {
            return View();
        }

        public IActionResult submit(Employee e1)
        {
            Employee e = new Employee();
            var regex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            bool isValid = Regex.IsMatch(e1.Email, regex, RegexOptions.IgnoreCase);
            
           
            string dateInput = e1.DateReg.ToString();          
            var parsedDate = DateTime.Parse(dateInput);
            DateTime from = new DateTime(2019, 1, 1);
            DateTime to = new DateTime(2019, 6, 30);
            
            if (parsedDate >= from && parsedDate < to && isValid==true)
            {
                
                e.Name = e1.Name.ToString();
                e.Email = e1.Email.ToString();
                e.Gender = e1.Gender.ToString();
                e.DateReg = parsedDate.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));

                if (e1.Day1 == true && e1.Day2 == true && e1.Day3 == true)
                    e.SelectedDays = "Day1,Day2,Day3";
                else if (e1.Day1 == true && e1.Day2 == true)
                    e.SelectedDays = "Day1,Day2";
                else if (e1.Day1 == true && e1.Day3 == true)
                    e.SelectedDays = "Day1,Day3";
                else if (e1.Day2 == true && e1.Day3 == true)
                    e.SelectedDays = "Day2,Day3";
                else if (e1.Day1 == true)
                    e.SelectedDays = "Day1";
                else if (e1.Day2 == true)
                    e.SelectedDays = "Day2";
                else if (e1.Day3 == true)
                    e.SelectedDays = "Day3";
                e.AdditionalRequest = e1.AdditionalRequest.ToString();
                dbcontext.Add(e);
                dbcontext.SaveChanges();
                return RedirectToAction("viewUsers");
            }
            else
            {
                ViewData["error"] = "Date is wrong";
                return RedirectToAction("addUser");
            }
                
        }


    }
}
