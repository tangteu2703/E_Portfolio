using DocumentFormat.OpenXml.Office2010.ExcelAc;
using E_Contract.Repository;
using E_Contract.Service;
using E_Contract.Service.WorkSheet;
using E_Model.Authentication;
using E_Model.Request.WorkSheet;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.WorkSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Service.WorkSheet
{
    public class WorkSheetService : ServiceBase<worksheet_daily>, IWorkSheetService
    {
        public WorkSheetService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.WorkSheet;
        }

        public async Task<IEnumerable<WorkSheetResponse>> SelectFilterAsync(WorkSheetRequest request)
        {
            return await _repositoryWrapper.WorkSheet.SelectFilterAsync(request);
        }

        public async Task<IEnumerable<worksheet_daily>> SelectAsync(DateTime date)
        {
            return await _repositoryWrapper.WorkSheet.SelectAsync(date);
        }

        public async Task<IEnumerable<worksheet_daily>> SelectLogicErrorAsync()
        {
            return await _repositoryWrapper.WorkSheet.SelectLogicErrorAsync();
        }

        public async Task<int> InsertBatchAsync(List<worksheet_daily> list, string type = "")
        {
            return await _repositoryWrapper.WorkSheet.InsertBatchAsync(list, type);
        }

        public async Task<int> UpdateBatchAsync(List<worksheet_daily> list, string type = "")
        {
            return await _repositoryWrapper.WorkSheet.UpdateBatchAsync(list, type);
        }

        public async Task<int> DeleteBatchAsync(string listId, string type = "")
        {
            return await _repositoryWrapper.WorkSheet.DeleteBatchAsync(listId, type);
        }

        //

        public async Task<IEnumerable<HR_BarCode>> SelectHR_BarcodeAsync(DateTime date, string db = "")
        {
            return await _repositoryWrapper.WorkSheet.SelectHR_BarcodeAsync(date, db);
        }
        public async Task<int> InsertUpdateBatchAsync(List<HR_BarCode> list, string type = "")
        {
            return await _repositoryWrapper.WorkSheet.InsertUpdateBatchAsync(list, type);
        }

        #region  tools
        public async Task<(bool success, string message)> Convert_WorkSheet_00_09(DateTime from, DateTime to)
        {
            try
            {
                var message = string.Empty;

                var listData = (await _repositoryWrapper.ClockTransaction.SelectTransactionDateAsync(from, to)).ToList();
                if (!listData.Any())
                    return (true, $"Không có dữ liệu từ {from} đến {to}.");

                var grouped = listData.GroupBy(x => x.emp_code);
                var listOne = grouped
                    .Where(g => g.Count() == 1)
                    .SelectMany(g => g)
                    .ToList();

                var listIn = listOne
                    .Where(x => x.punch_type.Contains("In"))
                    .Select(x => new worksheet_daily
                    {
                        Factory = "JCV",
                        Emp_Code = x.emp_code,
                        DateTime_In = x.punch_time,
                        Machine_In = x.terminal_sn,
                        Shift = x.punch_time.Hour < 2 ? "Ca đêm" : "Ca ngày",
                        LastTime_Modified = DateTime.Now,
                        User_ID = "TOOL",
                        Is_Deleted = false,
                    })
                    .OrderBy(x => x.DateTime_In)
                    .ToList();
                if (listIn.Any())
                {
                    var count = await _repositoryWrapper.WorkSheet.InsertBatchAsync(listIn, "in-001");
                    message += $"Đã insert {count} bản ghi từ listIn với in-001 vào bảng worksheet_daily.\n";
                }

                var listOut = listOne
                    .Where(x => x.punch_type.Contains("Out"))
                    .Select(x => new worksheet_daily
                    {
                        Factory = "JCV",
                        Emp_Code = x.emp_code,
                        DateTime_Out = x.punch_time,
                        Machine_Out = x.terminal_sn,
                        Shift = "Ca đêm",
                        LastTime_Modified = DateTime.Now,
                        User_ID = "TOOL",
                        Is_Deleted = false,
                    })
                    .OrderBy(x => x.DateTime_Out)
                    .ToList();
                if (listOut.Any())
                {
                    var count = await _repositoryWrapper.WorkSheet.UpdateBatchAsync(listOut, "out-002");
                    message += $"Đã update {count} bản ghi từ listOut với out-002 vào bảng worksheet_daily.\n";
                }

                var listMultiple = grouped
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .OrderByDescending(c => c.punch_type)
                    .Select(x => new worksheet_daily
                    {
                        Factory = "JCV",
                        Emp_Code = x.emp_code,
                        DateTime_In = x.punch_type.Contains("In") ? x.punch_time : null,
                        Machine_In = x.punch_type.Contains("In") ? x.terminal_sn : "",
                        DateTime_Out = x.punch_type.Contains("Out") ? x.punch_time : null,
                        Machine_Out = x.punch_type.Contains("Out") ? x.terminal_sn : "",
                        Shift = "",
                        LastTime_Modified = DateTime.Now,
                        User_ID = "TOOL",
                        Is_Deleted = false,
                    })
                    .ToList();
                if (listMultiple.Any())
                {
                    var count = await _repositoryWrapper.WorkSheet.UpdateBatchAsync(listMultiple, "out-004");
                    message += $"Đã insert+update {count} bản ghi từ listMultiple với out-004 vào bảng worksheet_daily.\n";
                }

                // Xử lý trường hợp sai về logic
                var listLogicError = (await _repositoryWrapper.WorkSheet.SelectLogicErrorAsync()).ToList();
                if (listLogicError.Any())
                {
                    var (success3, message3) = await ConvertLogicError(listLogicError);
                    message += $" Check LogicError: {message3}";
                }
                return (true, message);
            }
            catch (Exception ex)
            {
                // Trả kết quả false và thông tin lỗi
                return (false, ex.Message);
            }
        }
        public async Task<(bool success, string message)> Convert_WorkSheet_09_12(DateTime from, DateTime to)
        {
            try
            {
                var message = string.Empty;

                var listData = (await _repositoryWrapper.ClockTransaction.SelectTransactionDateAsync(from, to)).ToList();
                if (!listData.Any())
                {
                    return (true, $"Không có dữ liệu từ {from} đến {to}.");
                }

                var listMultiple = listData
                    .OrderBy(x => x.punch_type)
                    .Select(x => new worksheet_daily
                    {
                        Factory = "JCV",
                        Emp_Code = x.emp_code,
                        DateTime_In = x.punch_type.Contains("In") ? x.punch_time : null,
                        Machine_In = x.punch_type.Contains("In") ? x.terminal_sn : "",
                        DateTime_Out = x.punch_type.Contains("Out") ? x.punch_time : null,
                        Machine_Out = x.punch_type.Contains("Out") ? x.terminal_sn : "",
                        Shift = "",
                        LastTime_Modified = DateTime.Now,
                        User_ID = "TOOL",
                        Is_Deleted = false,
                    })
                    .ToList();

                if (listMultiple.Any())  // xử lý các bản ghi có nhiều lần vào ra
                {
                    var count = await _repositoryWrapper.WorkSheet.UpdateBatchAsync(listMultiple, "out-005");
                    message += $"Đã insert+update {count} bản ghi từ listMultiple với out-005 vào bảng worksheet_daily.\n";
                }

                return (true, message);
            }
            catch (Exception ex)
            {
                // Trả kết quả false và thông tin lỗi
                return (false, ex.Message);
            }
        }
        public async Task<(bool success, string message)> Convert_WorkSheet_12_24(DateTime from, DateTime to)
        {
            try
            {
                var message = string.Empty;
                var listData = (await _repositoryWrapper.ClockTransaction.SelectTransactionDateAsync(from, to)).ToList();
                if (!listData.Any())
                    return (true, $"Không có dữ liệu từ {from} đến {to}.");
                
                var grouped = listData.GroupBy(x => x.emp_code);
                var listOne = grouped
                    .Where(g => g.Count() == 1)
                    .SelectMany(g => g)
                    .ToList();

                var listIn = listOne
                    .Where(x => x.punch_type.Contains("In"))
                    .Select(x => new worksheet_daily
                    {
                        Factory = "JCV",
                        Emp_Code = x.emp_code,
                        DateTime_In = x.punch_time,
                        Machine_In = x.terminal_sn,
                        Shift = x.punch_time.Hour < 14 ? "Ca ngày" : "Ca đêm",
                        LastTime_Modified = DateTime.Now,
                        User_ID = "TOOL",
                        Is_Deleted = false
                    })
                    .OrderBy(x => x.DateTime_In)
                    .ToList();
                if (listIn.Any())
                {
                    var count = await _repositoryWrapper.WorkSheet.InsertBatchAsync(listIn, "in-002");
                    message += $"Đã insert {count} bản ghi từ listIn với in-002 vào bảng worksheet_daily.\n";
                }

                var listOut = listOne
                    .Where(x => x.punch_type.Contains("Out"))
                    .Select(x => new worksheet_daily
                    {
                        Factory = "JCV",
                        Emp_Code = x.emp_code,
                        DateTime_Out = x.punch_time,
                        Machine_Out = x.terminal_sn,
                        Shift = "Ca ngày",
                        LastTime_Modified = DateTime.Now,
                        User_ID = "TOOL",
                        Is_Deleted = false,
                    })
                    .ToList();
                if (listOut.Any())
                {
                    var count = await _repositoryWrapper.WorkSheet.UpdateBatchAsync(listOut, "out-001");
                    message += $"Đã update {count} bản ghi từ listOut với out-001 vào bảng worksheet_daily.\n";
                }

                var listMultiple = grouped
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .OrderBy(x => x.punch_type)
                    .Select(x => new worksheet_daily
                    {
                        Factory = "JCV",
                        Emp_Code = x.emp_code,
                        DateTime_In = x.punch_type.Contains("In") ? x.punch_time : null,
                        Machine_In = x.punch_type.Contains("In") ? x.terminal_sn : "",
                        DateTime_Out = x.punch_type.Contains("Out") ? x.punch_time : null,
                        Machine_Out = x.punch_type.Contains("Out") ? x.terminal_sn : "",
                        Shift = "",
                        LastTime_Modified = DateTime.Now,
                        User_ID = "TOOL",
                        Is_Deleted = false,
                    })
                    .ToList();
                if (listMultiple.Any())
                {
                    var count = await _repositoryWrapper.WorkSheet.UpdateBatchAsync(listMultiple, "out-003");
                    message += $"Đã insert+update {count} bản ghi từ listMultiple với out-003 vào bảng worksheet_daily.\n";
                }

                return (true, message);
            }
            catch (Exception ex)
            {
                // Trả về false kèm lỗi
                return (false, ex.Message);
            }
        }

        // Xử lý trường hợp sai về logic
        private async Task<(bool success, string message)> ConvertLogicError(List<worksheet_daily> listLogicError)
        {
            try
            {
                var message = string.Empty;

                // Điều kiện 1: Có từ 2 lần chấm công trong cùng một ngày
                // Điều kiện 2: Thời gian làm việc > 15 tiếng
                // Điều kiện 3: Thời gian làm việc< 1 tiếng
                // Điều kiện 4: In = Out day-1 => chấm công ra nhầm ngày hôm trước
                if (!listLogicError.Any())
                    return (true, "Không có dữ liệu sai!");
                else
                {
                    // Cùng ngày - ko có out -> in out nhiều
                    var listOutNull = listLogicError
                        .Where(c => c.DateTime_Out == null)
                        .ToList();
                    // Out - In >= 15 --> Out update nhầm ngày hôm trc
                    var listOver15 = listLogicError
                        .Where(c => c.DateTime_Out != null &&
                                    (c.DateTime_Out.Value - c.DateTime_In)?.TotalHours >= 15)
                        .ToList();
                    // Out - In < 1 --> ko tính công hôm đó
                    var listHaft1 = listLogicError
                        .Where(c => c.DateTime_Out != null &&
                                    (c.DateTime_Out.Value - c.DateTime_In)?.TotalHours < 1)
                        .ToList();

                    var listTotal = listOutNull
                                    .Concat(listOver15)
                                    .Concat(listHaft1)
                                    .ToList();

                    if (listTotal.Any())
                    {
                        var listId = string.Join(",", listTotal.Select(c => c.id));
                        var result = await _repositoryWrapper.WorkSheet.DeleteBatchAsync(listId, "out-010");
                        message += $"Đã deleted {result} bản ghi từ listLogicError với out-010 vào bảng worksheet_daily.\n";
                    }
                }

                return (true, message);
            }
            catch (Exception ex)
            {
                // Trả kết quả false và thông tin lỗi
                return (false, ex.Message);
            }
        }

        // Chuyển đổi dữ liệu từ WorkSheet sang HR_BarCode
        public async Task<(bool success, string message)> Convert_WorkSheet_to_HRBarcode(DateTime date, DateTime to)
        {
            try
            {
                var message = string.Empty;

                var listData = (await _repositoryWrapper.WorkSheet.SelectAsync(date))
                .Where(c => c != null)
                .OrderBy(c => c.DateTime_In)
                .ToList();

                if (!listData.Any()) return (true, "Không có dữ liệu !"); ;

                var now = DateTime.Now.ToString("yyyyMMddHHmmss");
                var userId = "ZTK-WorkSheet";
                var listHR = new List<HR_BarCode>();

                foreach (var item in listData)
                {
                    var emp = item.Emp_Code;
                    var fac = item.Factory;

                    // Tính ngày làm việc chuẩn từ IN, nếu không có thì từ OUT
                    var refTime = item.DateTime_In ?? item.DateTime_Out;
                    if (refTime == null) continue;

                    var dtWork = (item.DateTime_In?.Hour < 2 ? refTime.Value.AddDays(-1) : refTime.Value).ToString("yyyyMMdd");

                    void TryAdd(DateTime? dt, string wcode)
                    {
                        if (dt == null) return;

                        var time = dt.Value;

                        bool tooClose = listHR.Any(x =>
                            x.CD_COMPANY == fac && x.NO_CARD == emp && x.CD_WCODE == wcode &&
                            Math.Abs((DateTime.ParseExact(x.DT_WORK + x.TM_CARD, "yyyyMMddHHmmss", null) - time).TotalHours) < 3
                        );

                        if (!tooClose)
                        {
                            listHR.Add(new HR_BarCode
                            {
                                CD_COMPANY = fac,
                                NO_CARD = emp,
                                DT_WORK = time.ToString("yyyyMMdd"),
                                TM_CARD = time.ToString("HHmmss"),
                                CD_WCODE = wcode,
                                DTS_INSERT = now,
                                ID_INSERT = userId,
                                CD_USERDEF1 = dtWork
                            });
                        }
                    }

                    TryAdd(item.DateTime_In, "001");
                    TryAdd(item.DateTime_Out, "002");
                }

                // insert-update vào HR_BarCode
                var result = await _repositoryWrapper.WorkSheet.InsertUpdateBatchAsync(listHR, "001");
                message += $"Đã insert-update {result} bản ghi vào bảng HR_BarCode từ worksheet_daily.\n";

                return (true, message);
            }
            catch (Exception ex)
            {
                // Trả kết quả false và thông tin lỗi
                return (false, ex.Message);
            }
        }
        #endregion
    }
}
