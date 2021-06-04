using Spreadsheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.ViewModels
{
    public class IndexViewModel
    {
        public List<Sheet> Sheets { get; set; }
        public List<Sheet> SheetsFavorite { get; set; }
    
        public string UserNameToInv { get; set; }
    }
}
