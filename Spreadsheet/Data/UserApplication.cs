using Microsoft.AspNetCore.Identity;
using Spreadsheet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.Models
{

    public class UserApplication : IdentityUser
    {
        public UserApplication()
        {
            UserSheets = new List<Sheet>();
            UseSheetsFavoriteIds = new List<SheetFavorite>();
        }

     
        public List<Sheet> UserSheets { get; set; }
        public List<SheetFavorite> UseSheetsFavoriteIds { get; set; }

    }
}
