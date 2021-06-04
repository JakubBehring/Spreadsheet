using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.Models
{
    public class Sheet
    {
        public Sheet()
        {
            SheetValues = new List<SheetValue>();
            Users = new List<UserApplication>();
            Name = "spreadsheet#" + DateTime.Now.Ticks%175963; 
        }
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public List<SheetValue> SheetValues { get; set; }
        public List<UserApplication> Users { get; set; }
    }
}
