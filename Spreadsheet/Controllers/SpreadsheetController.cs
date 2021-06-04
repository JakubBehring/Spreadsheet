using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spreadsheet.Data;
using Spreadsheet.Models;
using Spreadsheet.Repositories;
using Spreadsheet.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet.Controllers
{
    [Authorize]
    public class SpreadsheetController : Controller
    {
        private readonly ILogger<SpreadsheetController> _logger;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<UserApplication> userManager;
        private readonly ISheetRepository sheetRepository;
        private readonly SheetValueInterpreter sheetValueInterpreter;
        private readonly IUserRepository userRepository;

        [BindProperty]
        public AddSheetViewModel AddSheetViewModelBinded { get; set; }
        [BindProperty]
        public IndexViewModel indexViewModel { get; set; }
        public UserApplication UserApplication { get; set; }


        public SpreadsheetController(ILogger<SpreadsheetController> logger, ApplicationDbContext applicationDbContext,
            UserManager<UserApplication> userManager, ISheetRepository sheetRepository, SheetValueInterpreter sheetValueInterpreter,
            IUserRepository userRepository)
        {
            _logger = logger;
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            this.sheetRepository = sheetRepository;
            this.sheetValueInterpreter = sheetValueInterpreter;
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> Index(string sheetName = "")
        {
            UserApplication = await userManager.GetUserAsync(User);
            var UserDb = userRepository.GetUserWithSheetsAndFavorites(UserApplication.Id);

            IEnumerable<Sheet> UserSheets = string.IsNullOrEmpty(sheetName) ?
                UserDb.UserSheets : UserDb.UserSheets.Where(s => s.Name.StartsWith(sheetName));

            var sheetsFavoriteIds = UserDb.UseSheetsFavoriteIds.Select(s => s.sheetId);
            IEnumerable<Sheet> UserSheetsFavorite = UserSheets.Where(s => sheetsFavoriteIds.Contains(s.ID));

            UserSheets = UserSheets.Except(UserSheetsFavorite);

            indexViewModel = new IndexViewModel() { Sheets = UserSheets.ToList(), SheetsFavorite =UserSheetsFavorite.ToList(), UserNameToInv = "" };

            return View(indexViewModel);
        }
        [AllowAnonymous]
        public IActionResult AllSheets(string sheetName = "")
        {
            IEnumerable<Sheet> allSheets = string.IsNullOrEmpty(sheetName) ?
             applicationDbContext.Sheets : applicationDbContext.Sheets.Where(s => s.Name.StartsWith(sheetName)).ToList();

            return View(new IndexViewModel() { Sheets = allSheets.ToList() });
        }
        [AllowAnonymous]
        public async Task<IActionResult> AddEditSheet(int sheetID = -1)
        {
            var sheet = new Sheet();
            var userOwnsSheet = false;
            UserApplication = await userManager.GetUserAsync(User);
            if (UserApplication != null)
            {
                var userDb = userRepository.GetUserWithSheetsAndValues(UserApplication.Id);
                userOwnsSheet = userDb.UserSheets.FirstOrDefault(s => s.ID == sheetID) != null || sheetID == -1;
                if (sheetID == -1)
                {
                    await sheetRepository.AddSheet(sheet);
                    userDb.UserSheets.Add(sheet);
                    await userManager.UpdateAsync(UserApplication);
                    await applicationDbContext.SaveChangesAsync();

                }
                else
                {
                    sheet = userDb.UserSheets.FirstOrDefault(s => s.ID == sheetID);
                }

            }
            else // inspect
            {
                sheet = sheetRepository.GetSheetByID(sheetID).Result;
                if (sheet == null)
                {
                    return NotFound();
                }
            }

            AddSheetViewModelBinded = new AddSheetViewModel()
            {
                SheetID = sheet.ID,
                sheetValue = new SheetValue(),
                SheetValues = sheet.SheetValues,
                SheetName = sheet.Name,
                userOwnsSheet = userOwnsSheet

            };
            return View(AddSheetViewModelBinded);
        }

        [HttpPost]
        public async Task<IActionResult> Check(int sheetID)
        {
            if (await UserHasClaimsToSheet(sheetID))
            {
                return NotFound();
            }

            string input = sheetValueInterpreter.Interprete(AddSheetViewModelBinded.sheetValue.Value, sheetID);

            AddSheetViewModelBinded.sheetValue.Value = input;
            AddSheetViewModelBinded.sheetValue.ID = AddSheetViewModelBinded.SheetValueID;
            sheetRepository.AddEditSheetValue(sheetID, AddSheetViewModelBinded.sheetValue);

            return RedirectToAction("AddEditSheet", new { sheetID = sheetID });
        }
        public async Task<IActionResult> InviteUser(int sheetID)
        {
            if (await UserHasClaimsToSheet(sheetID))
            {
                return NotFound();
            }
            UserApplication = userManager.Users.Include(u => u.UserSheets).Where(u => u.Email == indexViewModel.UserNameToInv).FirstOrDefault();
            var sheet = sheetRepository.GetSheetByID(sheetID).Result;
            string message = "something went wrong";
            if (UserApplication != null)
            {
                // user has that sheet already
                if (UserApplication.UserSheets.Contains(sheet))
                {
                    message = "this user already has access to this sheet";

                }
                else
                {
                    UserApplication.UserSheets.Add(sheet);
                    await userManager.UpdateAsync(UserApplication);
                    message = "User found and added to this sheet";
                }
            }
            else
            {
                message = "User not found";
            }
            return View("InviteUser", message);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateName(int sheetID)
        {
            await sheetRepository.UpdateName(sheetID, AddSheetViewModelBinded.SheetName);
            return RedirectToAction("AddEditSheet", new { sheetID = sheetID });
        }


        public async Task<IActionResult> DeleteSheet(int sheetID)
        {
            var SheetFound = await sheetRepository.GetSheetByID(sheetID);
            if (SheetFound == null || await UserHasClaimsToSheet(sheetID))
            {
                return NotFound();
            }
            await sheetRepository.DeleteSheet(sheetID);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ClearSheet(int sheetID)
        {
            if (await UserHasClaimsToSheet(sheetID))
            {
                return NotFound();
            }
            await sheetRepository.ClearSheetValue(sheetID);
            return RedirectToAction("AddEditSheet", new { sheetID = sheetID });
        }

        public async Task<IActionResult> AddSheetToFavorite(int sheetId)
        {
            if (await UserHasClaimsToSheet(sheetId))
            {
                return NotFound();
            }
            UserApplication = await userManager.GetUserAsync(User);

            var userDb = userRepository.GetUserWithSheetsFavorites(UserApplication.Id);
            if (userDb.UseSheetsFavoriteIds.FirstOrDefault(s => s.sheetId == sheetId) != null)
            {
                return RedirectToAction("Index");
            }
            userDb.UseSheetsFavoriteIds.Add(new SheetFavorite() { sheetId = sheetId });
            await userManager.UpdateAsync(UserApplication);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromFavorites(int sheetId)
        {
            if (await UserHasClaimsToSheet(sheetId))
            {
                return NotFound();
            }

            UserApplication = await userManager.GetUserAsync(User);
            var userDb = userRepository.GetUserWithSheetsFavorites(UserApplication.Id);
            
            var sheetFavorite = userDb.UseSheetsFavoriteIds.FirstOrDefault(s => s.sheetId == sheetId);
            userDb.UseSheetsFavoriteIds.Remove(sheetFavorite);
            applicationDbContext.Remove(sheetFavorite);
             await applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<bool> UserHasClaimsToSheet(int sheetId)
        {
            var userApplication = await userManager.GetUserAsync(User);
            var userDb= applicationDbContext.Users.Include(u => u.UserSheets).FirstOrDefault(u => u.Id == userApplication.Id);
            return userDb.UserSheets.FirstOrDefault(s => s.ID == sheetId) == null;

        }

    }
}




