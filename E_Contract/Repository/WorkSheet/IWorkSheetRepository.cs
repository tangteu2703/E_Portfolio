using E_Model.Request.WorkSheet;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.WorkSheet;

namespace E_Contract.Repository.WorkSheet
{
    public interface IWorkSheetRepository : IRepositoryBase<worksheet_daily>
    {
        Task<IEnumerable<WorkSheetResponse>> SelectFilterAsync(WorkSheetRequest request);
        Task<IEnumerable<worksheet_daily>> SelectAsync(DateTime date);
        Task<IEnumerable<worksheet_daily>> SelectLogicErrorAsync();
        Task<int> InsertBatchAsync(List<worksheet_daily> list, string type = "");
        Task<int> UpdateBatchAsync(List<worksheet_daily> list, string type = "");
        Task<int> DeleteBatchAsync(string listId, string type = "");


        //HR_BarCode
        Task<IEnumerable<HR_BarCode>> SelectHR_BarcodeAsync(DateTime date,string db = "");
        Task<int> InsertUpdateBatchAsync(List<HR_BarCode> list, string type = "");
    }
}
