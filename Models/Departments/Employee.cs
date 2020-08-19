using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Team7_StationeryStore.Models
{
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(30)]       
        public string Name { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }
       
        public string Gender { get; set; }
        public string DateReg { get; set; }

        public string SelectedDays { get; set; }
        public bool Day1 { get; set; }
        public bool Day2 { get; set; }
        public bool Day3 { get; set; }

        [MaxLength(100)]
        public string AdditionalRequest { get; set; }
    }
}
