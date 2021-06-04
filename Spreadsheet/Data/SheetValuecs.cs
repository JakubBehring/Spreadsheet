using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.Models
{
    public class SheetValue
    {
        [Key]
        public int ID { get; set; }
        public int SheetID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Value { get; set; }
        public Sheet Sheet { get; set; }
    }
}
