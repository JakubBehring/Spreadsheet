using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spreadsheet.Data;
using Spreadsheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spreadsheet.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<UserApplication> userManager;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public UserApplication GetUserWithSheets(string userID)
             => applicationDbContext.Users
            .Include(u => u.UserSheets)
            .FirstOrDefault(u => u.Id == userID);

        public UserApplication GetUserWithSheetsAndFavorites(string userID)
           => applicationDbContext.Users
          .Include(u => u.UserSheets)
          .Include(u => u.UseSheetsFavoriteIds)
          .FirstOrDefault(u => u.Id == userID);
        public UserApplication GetUserWithSheetsAndValues(string userID)
            => applicationDbContext.Users
            .Include(u => u.UserSheets)
            .ThenInclude(s => s.SheetValues)
            .FirstOrDefault(u => u.Id == userID);

        public UserApplication GetUserWithSheetsFavorites(string userID) =>
            applicationDbContext.Users.Include(u => u.UseSheetsFavoriteIds)
            .FirstOrDefault(u => u.Id == userID);

        public UserApplication GetUserWithSheetsAndValuesAndFavorites(string userID)
             => applicationDbContext.Users
            .Include(u => u.UseSheetsFavoriteIds)
            .Include(u => u.UserSheets)
            .ThenInclude(s => s.SheetValues)
            .FirstOrDefault(u => u.Id == userID);


    }
}
