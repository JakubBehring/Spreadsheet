using Spreadsheet.Models;
using System.Threading.Tasks;

namespace Spreadsheet.Repositories
{
    public interface ISheetRepository
    {
        Task AddSheet(Sheet sheet);
        Task<Sheet> GetSheetByID(int id);
        void AddEditSheetValue(int idSheet, SheetValue sheetValue);
        public  Task UpdateName(int id, string name);
        public Task DeleteSheet(int id);
        public Task ClearSheetValue(int sheetId);
    }
}