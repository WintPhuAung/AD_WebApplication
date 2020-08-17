﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Team7_StationeryStore.Models;
using Team7_StationeryStore.Database;
using Microsoft.AspNetCore.Http;
using Team7_StationeryStore.Service;

namespace Team7_StationeryStore.Controllers
{
    public class StationeryStoreController : Controller
    {
        protected StationeryContext dbcontext;
        protected RetrievalService rservice;
        protected RequisitionService requisitionService;
        protected InventoryService InventoryService;

        public StationeryStoreController(StationeryContext dbcontext, RetrievalService rservice,RequisitionService requisitionService)
        {
            this.dbcontext = dbcontext;
            this.rservice = rservice;
            this.requisitionService = requisitionService;
        }

        public IActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("Reached Stationery Store Controller");
            return RedirectToAction("viewRetrieval", "StationeryStore");
        }

        public IActionResult viewRetrieval(List<string> req)
        {
            List<Requisition> selectedReq=new List<Requisition>();
            foreach (string value in req)
            {
                Requisition r = requisitionService.findRequisition(value);
                selectedReq.Add(r);
            }
            //Replace with the selected Requisitions from RequisitionController
           // List<Requisition> selectedReq = (from x in dbcontext.requisitions
                                            //select x).ToList();

            //Retrieval List Generation Starts here
            List<RequisitionDetail> selectedReqDetail = rservice.getRequisitionDetail(selectedReq);          
            Dictionary<string, int> totalItemQty = rservice.getTotalQtyPerItem(selectedReqDetail);
            Dictionary<string, List<RequisitionDetail>> reqPerIt = rservice.getReqDetailPerItem(selectedReqDetail);
            ViewData["totalItemQty"] = totalItemQty;
            rservice.recommendQty(reqPerIt);
            Dictionary<string, List<RequisitionDetail>> reqPerDept = rservice.getReqPerDeptPerItem(reqPerIt);
            ViewData["reqPerDept"] = reqPerDept;
            return View();        
        }

        public IActionResult submitAdjustmentVoucher(string invId,int qty,string reason) {
            string userid = HttpContext.Session.GetString("userId");
            InventoryService.CreateAdjustmentVoucher(userid, invId, qty, reason);
            return RedirectToAction("viewInventoryList");
        }

        public IActionResult updateAdjustmentVoucher(string adjVoucherId,string action,string remarks) {
            string userId = HttpContext.Session.GetString("userId");
            ViewData["response"] = InventoryService.UpdateAdjustmentVoucher(adjVoucherId, action, remarks);
            return RedirectToAction("");
        }
        
    }
}
