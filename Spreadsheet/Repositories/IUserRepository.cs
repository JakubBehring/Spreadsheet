using Spreadsheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.Repositories
{
   public interface IUserRepository
    {
        UserApplication GetUserWithSheets(string userID);
        UserApplication GetUserWithSheetsAndFavorites(string userID);
        UserApplication GetUserWithSheetsAndValues(string userID);
        UserApplication GetUserWithSheetsFavorites(string userID);
        UserApplication GetUserWithSheetsAndValuesAndFavorites(string userID);


    }
}
