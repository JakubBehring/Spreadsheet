using Spreadsheet.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.Data
{
    public class SheetFavorite
    {
        public SheetFavorite()
        {
            UserApplications = new List<UserApplication>();
        }
        [Key]
        public int Id { get; set; }
        public int sheetId { get; set; }
       public List<UserApplication> UserApplications { get; set; }

    }
}
