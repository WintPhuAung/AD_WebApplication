﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Team7_StationeryStore.Models
{
    public enum Status
    {
        APPROVED, PENDING
    }

    public class AdjustmentVoucher
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        [Required]
        public string InventoryId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        /*        [Required]
                public string reqEmployeeId { get; set; }*/

        //Beaware of this
        /*        [Required]
                public string approvedEmployeeId { get; set; }*/
        public int qty { get;  set; }
        public DateTime date { get;  set; }
        public string reason { get;  set; }
        public Status status { get;  set; }

/*        [ForeignKey("reqEmployeeId")]*/
        public virtual Employee Employee { get;  set; }
/*        [ForeignKey("approvedEmployeeId")]
        public virtual Employee ApprovedEmployee { get; set; }*/
        public virtual Inventory Inventory { get; set; }

    }
}