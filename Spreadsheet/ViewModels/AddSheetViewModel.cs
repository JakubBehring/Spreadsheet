using Spreadsheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.ViewModels
{
    public class AddSheetViewModel
    { 
        public int SheetID { get; set; }
        public int SheetValueID { get; set; }
        public SheetValue sheetValue { get; set; }
        public List<SheetValue> SheetValues { get; set; }
        public string SheetName { get; set; }
        public bool userOwnsSheet { get; set; }
     
    }
}
