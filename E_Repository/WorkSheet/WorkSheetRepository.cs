using Dapper;
using E_Common;
using E_Contract.Repository.WorkSheet;
using E_Model.Request.WorkSheet;
using E_Model.Response;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.WorkSheet;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using static E_Common.myExtension;

namespace E_Repository.WorkSheet
{
    public class WorkSheetRepository : RepositoryBase<worksheet_daily>, IWorkSheetRepository
    {
        public async Task<IEnumerable<WorkSheetResponse>> SelectFilterAsync(WorkSheetRequest request)
        {
            try
            {
                var param = request.ToDynamicParameters();

                var result = await Connection.SelectAsync<WorkSheetResponse>("worksheet_daily_select_filter", param, "HRMConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DataTableResponse<WorkSheetResponse>> SelectFilterAsync(WorkSheetRequest request, string version = "")
        {
            try
            {
                var data = new DataTableResponse<WorkSheetResponse>();

                var param = request.ToDynamicParameters();
                param.Add("@recordsTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@recordsFiltered", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var storeName = "worksheet_daily_select_filter_v2";
                if (version == "zktbio")
                    storeName = "iclock_transaction_select_filter";

                var result = await Connection.SelectAsync<WorkSheetResponse>(
                    storeName,
                    param,
                    "HRMConnection"
                );

                data.recordsTotal = param.Get<int>("@recordsTotal");
                data.recordsFiltered = param.Get<int>("@recordsFiltered");
                data.listData = result;

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<DataTableResponse<worksheet_detail>> SelectDetailAsync(WorkSheetRequest request, string version = "")
        {
            try
            {
                var data = new DataTableResponse<worksheet_detail>();

                var param = request.ToDynamicParameters();
                param.Add("@recordsTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@recordsFiltered", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var storeName = "worksheet_daily_select_filter_detail";
                var result = await Connection.SelectAsync<worksheet_detail>(
                    storeName,
                    param,
                    "HRMConnection"
                );

                data.recordsTotal = param.Get<int>("@recordsTotal");
                data.recordsFiltered = param.Get<int>("@recordsFiltered");
                data.listData = result;

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<DataTableResponse<WorkSheetTimeRespone>> SelectTimeSheetAsync(WorkSheetRequest request, string version = "")
        {
            try
            {
                var data = new DataTableResponse<WorkSheetTimeRespone>();

                var param = request.ToDynamicParameters();
                param.Add("@recordsTotal", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@recordsFiltered", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var storeName = "WorkSheetTime_select_filter_detail";
                var result = await Connection.SelectAsync<WorkSheetTimeRespone>(
                    storeName,
                    param
                );
                // tính total cho từng record
                foreach (var item in result)
                {
                    item.total =
                        (item.day_01 ?? 0) + (item.day_02 ?? 0) + (item.day_03 ?? 0) + (item.day_04 ?? 0) +
                        (item.day_05 ?? 0) + (item.day_06 ?? 0) + (item.day_07 ?? 0) + (item.day_08 ?? 0) +
                        (item.day_09 ?? 0) + (item.day_10 ?? 0) + (item.day_11 ?? 0) + (item.day_12 ?? 0) +
                        (item.day_13 ?? 0) + (item.day_14 ?? 0) + (item.day_15 ?? 0) + (item.day_16 ?? 0) +
                        (item.day_17 ?? 0) + (item.day_18 ?? 0) + (item.day_19 ?? 0) + (item.day_20 ?? 0) +
                        (item.day_21 ?? 0) + (item.day_22 ?? 0) + (item.day_23 ?? 0) + (item.day_24 ?? 0) +
                        (item.day_25 ?? 0) + (item.day_26 ?? 0) + (item.day_27 ?? 0) + (item.day_28 ?? 0) +
                        (item.day_29 ?? 0) + (item.day_30 ?? 0) + (item.day_31 ?? 0);
                }

                data.recordsTotal = param.Get<int>("@recordsTotal");
                data.recordsFiltered = param.Get<int>("@recordsFiltered");
                data.listData = result.OrderBy(x=>x.user_code).ToList();

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<TransactionResponse>> SelectBioHistoryAsync(WorkSheetRequest request)
        {
            try
            {
                var param = request.ToDynamicParameters();

                var result = await Connection.SelectAsync<TransactionResponse>("iclock_transaction_select_filter", param, "HRMConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<worksheet_daily>> SelectAsync(DateTime date)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@date", date);
                var result = await Connection.SelectAsync<worksheet_daily>("worksheet_daily_select_date", param, "HRMConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<worksheet_daily>> SelectAsync(DateTime from, DateTime to)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@from", from);
                param.Add("@to", to);
                var result = await Connection.SelectAsync<worksheet_daily>("worksheet_daily_select_date2", param, "HRMConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<worksheet_daily>> SelectToDayAsync(DateTime from, DateTime to)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@from", from);
                param.Add("@to", to);
                var result = await Connection.SelectAsync<worksheet_daily>("worksheet_daily_select_date3", param, "HRMConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<worksheet_daily>> SelectLogicErrorAsync()
        {
            try
            {
                var param = new DynamicParameters();
                var result = await Connection.SelectAsync<worksheet_daily>("worksheet_daily_select_logic_error", param, "HRMConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> InsertBatchAsync(List<worksheet_daily> list, string type = "")
        {
            var stroreName = $"";
            switch (type)
            {
                case "in-001":
                    stroreName = $"worksheet_daily_insert_batch_in_day";
                    break;
                case "in-002":
                    stroreName = $"worksheet_daily_insert_batch_in_night";
                    break;
                default:
                    stroreName = "";
                    break;
            }
            try
            {
                // Chuyển List<T> thành DataTable
                var dataTable = TableTypeConverter.ConvertToDataTable(list);
                var typeName = $"WorkSheet_Daily_Type";

                var param = new DynamicParameters();
                param.Add("@WorkSheets", dataTable.AsTableValuedParameter(typeName));
                param.Add("@insertedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Gọi store procedure, ví dụ: WorkSheetDaily_insert_batch
                var result = await Connection.ExcuteScalarAsync(stroreName, param, "insertedCount", "HRMConnection");

                return result; // Số lượng insert thành công
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> UpdateBatchAsync(List<worksheet_daily> list, string type = "")
        {
            var stroreName = $"";
            switch (type)
            {
                case "out-001":
                    stroreName = $"worksheet_daily_update_batch_out_day";
                    break;
                case "out-002":
                    stroreName = $"worksheet_daily_update_batch_out_night";
                    break;
                case "out-003": // Insert In và Out ca ngày
                    stroreName = $"worksheet_daily_in_out_day";
                    break;
                case "out-004": // Insert In và Out ca đêm
                    stroreName = $"worksheet_daily_in_out_night";
                    break;
                case "out-005": // Insert In và Out khung giờ 09-12
                    stroreName = $"worksheet_daily_in_out_middle";
                    break;
                default:
                    stroreName = "";
                    break;
            }
            try
            {
                var dataTable = TableTypeConverter.ConvertToDataTable(list);

                var typeName = "WorkSheet_Daily_Type"; // Tên TYPE trong SQL

                var param = new DynamicParameters();
                param.Add("@WorkSheets", dataTable.AsTableValuedParameter(typeName));
                param.Add("@UpdatedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await Connection.ExecuteAsync(stroreName, param, "HRMConnection");

                int updatedCount = param.Get<int>("@UpdatedCount");

                return updatedCount;
            }
            catch (Exception ex)
            {
                // Có thể ghi log nếu cần
                throw;
            }
        }
        public async Task<int> DeleteBatchAsync(string listId, string type = "")
        {
            var stroreName = $"";
            switch (type)
            {
                case "out-010":
                    stroreName = $"worksheet_daily_delete_batch_out";
                    break;
                case "out-011":
                    stroreName = $"";
                    break;
            }
            try
            {
                var param = new DynamicParameters();
                param.Add("@listId", listId);
                param.Add("@DeletedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await Connection.ExecuteAsync(stroreName, param, "HRMConnection");

                int updatedCount = param.Get<int>("@DeletedCount");

                return updatedCount;
            }
            catch (Exception ex)
            {
                // Có thể ghi log nếu cần
                throw;
            }
        }


        public async Task<int> UpdateTimeSheetBatchAsync(List<WorkSheetTime> list, string type = "")
        {
            var dataTable = TableTypeConverter.ConvertToDataTable(list);

            var typeName = "WorkSheetTime_Type"; // Tên TYPE trong SQL

            var param = new DynamicParameters();
            param.Add("@WorkSheets", dataTable.AsTableValuedParameter(typeName));
            param.Add("@UpdatedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var response =  await Connection.ExecuteAsync("WorkSheetTime_Insert_Update", param);

            int updatedCount = param.Get<int>("@UpdatedCount");

            return updatedCount;
        }

        #region HR_BarCode
        public async Task<IEnumerable<HR_BarCode>> SelectHR_BarcodeAsync(DateTime date, string db = "")
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@date", date.ToString("yyyyMMdd"));
                var result = await Connection.SelectAsync<HR_BarCode>("hr_barcode_select_date", param, db);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> InsertUpdateBatchAsync(List<HR_BarCode> list, string type = "")
        {
            var stroreName = $"";
            switch (type)
            {
                case "001":
                    stroreName = $"hr_barcode_insert_update_batch";
                    break;
                case "002":
                    stroreName = $"";
                    break;
                default:
                    stroreName = "";
                    break;
            }
            try
            {
                // Chuyển List<T> thành DataTable
                var dataTable = TableTypeConverter.ConvertToDataTable(list);
                var typeName = $"HR_BarCode_Type";

                var param = new DynamicParameters();
                param.Add("@list", dataTable.AsTableValuedParameter(typeName));
                param.Add("@count", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Gọi store procedure, ví dụ: WorkSheetDaily_insert_batch
                var result = await Connection.ExcuteScalarAsync(stroreName, param, "count", "HRMConnection");

                return result; // Số lượng insert thành công
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region [WorkSheet_Detail]
        public async Task<(int Inserted, int Updated)> InsertUpdate_DetailAsync(List<worksheet_detail> list, string type = "")
        {
            var stroreName = $"";
            switch (type)
            {
                case "001":
                    stroreName = $"worksheet_detail_insert_update_batch";
                    break;
                default:
                    stroreName = "";
                    break;
            }
            try
            {
                // Chuyển List<T> thành DataTable
                var dataTable = TableTypeConverter.ConvertToDataTable(list);
                var typeName = $"WorkSheet_Detail_Type";

                var param = new DynamicParameters();
                param.Add("@list", dataTable.AsTableValuedParameter(typeName));

                param.Add("@inserted", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@updated", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Gọi store procedure
                var outputNames = new[] { "@inserted", "@updated" };
                var result = await Connection.ExecuteScalarOutputsAsync(stroreName, param, outputNames, "HRMConnection");

                return (result["@inserted"], result["@updated"]);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

    }

}
