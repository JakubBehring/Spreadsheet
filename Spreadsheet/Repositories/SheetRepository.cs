using Microsoft.AspNetCore.Identity;
using Spreadsheet.Data;
using Spreadsheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Spreadsheet.Repositories
{
    public class SheetRepository : ISheetRepository
    {
        private readonly ApplicationDbContext applicationDbContext;


        public SheetRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;

        }

        public async Task AddSheet(Sheet sheet)
        {

            if (sheet != null)
            {
                await applicationDbContext.Sheets.AddAsync(sheet);
                await applicationDbContext.SaveChangesAsync();
            }
        }

        public void AddEditSheetValue(int idSheet, SheetValue sheetValue)
        {
          
            if (sheetValue.ID <= 0)
            {
                if (!string.IsNullOrEmpty(sheetValue.Value))
                {
                    sheetValue.SheetID = idSheet;
                    applicationDbContext.sheetValues.Add(sheetValue);
                }

            }
            else 
            {
                var sheetValueToEdit = applicationDbContext.sheetValues.Find(sheetValue.ID);
                if (string.IsNullOrWhiteSpace(sheetValue.Value))
                {
                    applicationDbContext.sheetValues.Remove(sheetValueToEdit);
                }
                else
                {
                    sheetValueToEdit.Value = sheetValue.Value;
                }


            }
            applicationDbContext.SaveChanges();

        }
        public async Task ClearSheetValue(int sheetId)
        {
            var allIDs = applicationDbContext.sheetValues.Where(v => v.SheetID == sheetId);
            applicationDbContext.sheetValues.RemoveRange(allIDs);

          await  applicationDbContext.SaveChangesAsync();

        }
        public async Task UpdateName(int id, string name)
        {
            Sheet sheetToUpdate = await applicationDbContext.Sheets.FindAsync(id);
            sheetToUpdate.Name = name;
            await applicationDbContext.SaveChangesAsync();

        }
        public async Task<Sheet> GetSheetByID(int id)
        {
            Sheet sheetToReturn = await applicationDbContext.Sheets
                .Include(s => s.SheetValues)
                .FirstOrDefaultAsync(s => s.ID == id);
            return sheetToReturn;
        }

        public async Task DeleteSheet(int id)
        {
            var sheetFound = await applicationDbContext.Sheets.FindAsync(id);

            applicationDbContext.Sheets.Remove(sheetFound);
            
          await  applicationDbContext.SaveChangesAsync();
        }
    }
}
