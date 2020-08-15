﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Team7_StationeryStore.Models;
using Team7_StationeryStore.Database;
using Microsoft.AspNetCore.Http;
using Team7_StationeryStore.Service;

namespace Team7_StationeryStore.Controllers
{
    public class DepartmentController : Controller
    {
        protected StationeryContext dbcontext;
        protected RequisitionService reqService;
        protected InventoryService invService;
        protected DepartmentService deptService;


        public DepartmentController(RequisitionService reqService, InventoryService invService, DepartmentService deptService ,StationeryContext dbcontext)
        {
            this.deptService = deptService;
            this.invService = invService;
            this.reqService = reqService;
            this.dbcontext = dbcontext;
        }

        public IActionResult Index(string userid)
        {
            Employee employee = dbcontext.employees.Where(x => x.Id == userid).FirstOrDefault();

            return View();
        }

        public IActionResult viewCatalogue()
        {
            List<Inventory> stationeryCatalogue = invService.retrieveCatalogue();
            List<ItemCategory> categories = invService.retrieveCategories();
            ViewData["stationeryCatalgoue"] = stationeryCatalogue;
            ViewData["categories"] = categories;
            return View();
        }

        [HttpPost]
        public IActionResult Search(string SearchString, string? userid)
        {
            List<ItemCategory> categories = invService.retrieveCategories();
            List<Inventory> items = new List<Inventory>();

            if (!String.IsNullOrEmpty(SearchString))
            {
                items = dbcontext.inventories.Where(s => s.itemCode.Contains(SearchString) || s.description.Contains(SearchString)).ToList();
            }
            ViewData["categories"] = categories;
            ViewData["stationeryCatalgoue"] = items;
            ViewData["userid"] = userid;
            return View("viewCatalogue");
        }

        public IActionResult RaiseRequisition() {
            reqService.CreateRequisition(HttpContext.Session.GetString("userId"));
            return RedirectToAction("viewRequisitionList", "Department");
        }

        /*public IActionResult AddToCart(string itemId,int qty) {
            return RedirectToAction("");
        }*/

        public IActionResult AddToCart(string itemId, int quantity)
        {
            string userid = HttpContext.Session.GetString("userId");
            var User = dbcontext.employees.Where(x => x.Id == userid).FirstOrDefault();
            AddItem(userid, itemId, quantity);
            return RedirectToAction("viewCatalogue");
        }
        public IActionResult ViewCart()
        {
            string userid = HttpContext.Session.GetString("userId");
            List<EmployeeCart> employeeCarts = reqService.retrieveEmployeeCart(userid);
            ViewData["employeeCarts"] = employeeCarts;
            return View();
        }
        public void AddItem(string userid, string itemid, int qty)
        {
            var oldcartItem = dbcontext.employeeCarts
                .Where(x => x.EmployeeId == userid && x.InventoryId == itemid)
                .FirstOrDefault();
            if (oldcartItem == null)
            {
                var cartItem = new EmployeeCart()
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployeeId = userid,
                    InventoryId = itemid,
                    Qty = qty,
                    Inventory = dbcontext.inventories.SingleOrDefault(p => p.Id == itemid)
                };
                dbcontext.employeeCarts.Add(cartItem);
                dbcontext.SaveChanges();

            }
            else
            {
                oldcartItem.Qty = qty;
                dbcontext.Update(oldcartItem);
                dbcontext.SaveChanges();
            }
        }

        public IActionResult UpdateQty(string itemId, int newQty)
        {
            int newqty = newQty;
            string itemid = itemId;
            string user = HttpContext.Session.GetString("userid");

            var cartItem = dbcontext.employeeCarts
                .Where(x => x.EmployeeId == user && x.InventoryId == itemid)
                .FirstOrDefault();

            if (cartItem != null)
            {
                if (newqty > 0)
                {
                    cartItem.Qty = newqty;
                }
                else
                {
                    RemoveItem(user, itemid);
                }
            }
            dbcontext.SaveChanges();
            return RedirectToAction("viewRequisition");
        }

        public void RemoveItem(string userid, string itemId)
        {
            var cartItem = dbcontext.employeeCarts
                .Where(x => x.EmployeeId == userid && x.InventoryId == itemId)
                .FirstOrDefault();
            if (cartItem != null)
            {
                dbcontext.employeeCarts.Remove(cartItem);
            }
        }

        public IActionResult viewRequisitionList() {
            string userId = HttpContext.Session.GetString("userId");
            Employee employee = deptService.findEmployeeById(userId);
            List<Requisition> Requisition = reqService.retrieveRequisitionByEmployee(employee);
            ViewData["Requisitions"] = Requisition;
            return View();
        }

        public IActionResult viewRequisition(string requisitionId)
        {
            string userId = HttpContext.Session.GetString("userId");
            Requisition requisition = reqService.findRequisition(requisitionId);
            ViewData["Requisition"] = requisition;
            TempData["UserId"] = userId;
            return View();
        }

        public IActionResult approveRequisition(string requisitionId) {
            string userId = HttpContext.Session.GetString("userId");
            Requisition requisition = dbcontext.requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();
            if (requisition == null) {
                ViewData["ErrorMsg"] = "Failed to locate requisition";    
                return RedirectToAction("ViewPendingRequisition");
            }
            if (requisition.EmployeeId == userId) {
                ViewData["ErrorMsg"] = "Cannot self-approve requisition";
                return RedirectToAction("ViewPendingRequisition");
            }
            requisition.status = ReqStatus.APPROVED;
            dbcontext.Update(requisition);
            dbcontext.SaveChanges();
            return RedirectToAction();       
        }

        public IActionResult rejectRequisition(string requisitionId) {
            string userId = HttpContext.Session.GetString("userId");
            Requisition requisition = dbcontext.requisitions.Where(x => x.Id == requisitionId).FirstOrDefault();
            requisition.status = ReqStatus.REJECTED;
            dbcontext.Update(requisition);
            dbcontext.SaveChanges();
            return View();

        }

        public IActionResult delegateAuthority() {
            string userId = HttpContext.Session.GetString("userId");
            List<Employee> deptEmployees = deptService.findDepartmentEmployeeList(userId);
            TempData["UserId"] = userId;
            ViewData["Employees"] = deptEmployees;
            return View();
        }

        public IActionResult submitDelegation(IFormCollection form) {

            var userId = TempData["UserId"];
            string Name = form["employeeName"];
            string SD = form["startDate"];
            string ED = form["endDate"];
            if (deptService.IsinvalidDate(SD,ED)) {
                ViewData["DateError"] = "Wrong Input Date";
                return RedirectToAction("DelegateAuthority");
            }
            deptService.createEmployeeAuthorization(Name, SD, ED);
            return RedirectToAction("DelegateAuthority");
        }

        
    }
}
