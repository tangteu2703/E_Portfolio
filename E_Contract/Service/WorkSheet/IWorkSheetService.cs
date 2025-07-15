using E_Model.Request.WorkSheet;
using E_Model.Response;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.WorkSheet;

namespace E_Contract.Service.WorkSheet
{
    public interface IWorkSheetService : IServiceBase<worksheet_daily>
    {
        #region  tools
        Task<(bool success, string message)> Convert_WorkSheet_00_09(DateTime from, DateTime to);
        Task<(bool success, string message)> Convert_WorkSheet_09_12(DateTime from, DateTime to);
        Task<(bool success, string message)> Convert_WorkSheet_12_24(DateTime from, DateTime to);
        Task<(bool success, string message)> Convert_WorkSheet_to_HRBarcode(DateTime from, DateTime to, int ver = 1);
        Task<(bool success, string message)> Convert_WorkSheet_to_Detail(DateTime from, DateTime to);

        #endregion
        Task<IEnumerable<WorkSheetResponse>> SelectFilterAsync(WorkSheetRequest request);
        Task<DataTableResponse<WorkSheetResponse>> SelectFilterAsync(WorkSheetRequest request, string version = "");
        Task<DataTableResponse<worksheet_detail>> SelectDetailAsync(WorkSheetRequest request, string version = "");
        Task<IEnumerable<TransactionResponse>> SelectBioHistoryAsync(WorkSheetRequest request);
        Task<IEnumerable<worksheet_daily>> SelectAsync(DateTime date);
        Task<IEnumerable<worksheet_daily>> SelectLogicErrorAsync();

        Task<int> InsertBatchAsync(List<worksheet_daily> list, string type = "");
        Task<int> UpdateBatchAsync(List<worksheet_daily> list, string type = "");
        Task<int> DeleteBatchAsync(string listId, string type = "");

        //HR_BarCode
        Task<IEnumerable<HR_BarCode>> SelectHR_BarcodeAsync(DateTime date, string db = "");
        Task<int> InsertUpdateBatchAsync(List<HR_BarCode> list, string type = "");

    }
}
