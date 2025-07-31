using E_Contract.Repository;
using E_Contract.Service.WorkSheet;
using E_Model.Request.WorkSheet;
using E_Model.Response;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.WorkSheet;

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

        public async Task<DataTableResponse<WorkSheetResponse>> SelectFilterAsync(WorkSheetRequest request, string version = "")
        {
            return await _repositoryWrapper.WorkSheet.SelectFilterAsync(request, version);
        }

        public async Task<DataTableResponse<worksheet_detail>> SelectDetailAsync(WorkSheetRequest request, string version = "")
        {
            return await _repositoryWrapper.WorkSheet.SelectDetailAsync(request, version);
        }

        public async Task<IEnumerable<TransactionResponse>> SelectBioHistoryAsync(WorkSheetRequest request)
        {
            return await _repositoryWrapper.WorkSheet.SelectBioHistoryAsync(request);
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
                    var a = listLogicError.Where(c => c.Emp_Code == "J00005").ToList();

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
                // Điều kiện 2: Thời gian làm việc > 18 tiếng
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
                                    (c.DateTime_Out.Value - c.DateTime_In)?.TotalHours >= 18)
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
        public async Task<(bool success, string message)> Convert_WorkSheet_to_HRBarcode(DateTime from, DateTime to, int ver = 1)
        {
            try
            {
                // Xử lý trường hợp sai về logic
                var listLogicError = (await _repositoryWrapper.WorkSheet.SelectLogicErrorAsync()).ToList();
                if (listLogicError.Any())
                {
                    var (success3, message3) = await ConvertLogicError(listLogicError);
                }

                var message = string.Empty;
                var listData = new List<worksheet_daily>();

                if (ver == 2)
                    listData = (await _repositoryWrapper.WorkSheet.SelectAsync(from, to)).ToList();
                else
                    listData = (await _repositoryWrapper.WorkSheet.SelectAsync(from)).ToList();

                // Lọc và sắp xếp dữ liệu
                listData = listData.Where(c => c != null)
                .OrderBy(c => c.DateTime_In)
                .ToList();

                if (!listData.Any()) return (true, "Không có dữ liệu !");

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

        // Chuyển đổi dữ liệu từ WorkSheet sang bảng chi tiết tính công
        public async Task<(bool success, string message)> Convert_WorkSheet_to_Detail(DateTime from, DateTime to)
        {
            try
            {
                var message = string.Empty;

                var listData = (await _repositoryWrapper.WorkSheet.SelectToDayAsync(from, to))
                .OrderBy(c => c.DateTime_In)
                .ToList();

                if (!listData.Any()) return (true, "Không có dữ liệu !");

                var holidayList = GenerateHolidayList(from);
                var listDetail = CalculateWorksheetDetail_v2(from, listData, holidayList);

                // insert-update vào WorkSheet_Detail
                var result = await _repositoryWrapper.WorkSheet.InsertUpdate_DetailAsync(listDetail, "001");
                message += $"Đã insert {result.Inserted} - update {result.Updated} bản ghi vào bảng WorkSheet_Detail từ worksheet_daily.\n";

                return (true, message);
            }
            catch (Exception ex)
            {
                // Trả kết quả false và thông tin lỗi
                return (false, ex.Message);
            }
        }
        private float RoundDownToNearestHalf(float hours) => (float)(Math.Floor(hours * 2) / 2);
        private float RoundDownToInt(float hours) => (float)Math.Floor(hours) > 0 ? (float)Math.Floor(hours) : 0;

        private static int GetWeekOfYear(DateTime date) =>
        System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
        date,
        System.Globalization.CalendarWeekRule.FirstFourDayWeek,
        DayOfWeek.Monday);

        public List<(DateTime date, string note)> GenerateHolidayList(DateTime date)
        {
            var holidayList = new List<(DateTime, string)>();

            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                // Chủ nhật
                if (d.DayOfWeek == DayOfWeek.Sunday)
                {
                    holidayList.Add((d, "Weekend"));
                    continue;
                }

                // Thứ 7 tuần lẻ
                if (d.DayOfWeek == DayOfWeek.Saturday && GetWeekOfYear(d) % 2 == 1)
                {
                    holidayList.Add((d, "Weekend"));
                    continue;
                }

                // Ngày lễ cố định
                if ((d.Month == 1 && d.Day == 1) ||   // Tết dương
                    (d.Month == 4 && d.Day == 30) ||  // 30/4
                    (d.Month == 5 && d.Day == 1) ||   // 1/5
                    (d.Month == 9 && d.Day == 2))     // fake tạm ngày lễ
                {
                    holidayList.Add((d, "Holiday"));
                }
            }

            return holidayList;
        }


        #region /// tính toán chi tiết công
        //check ngày làm việc
        //1. ngày thường
        //    ca làm chính 08:00 - 17:00 và 20:00 - 05:00
        //    trước 8h, sau 17h -> 22h, trước 20h tính giờ xét value vào cột OT_101
        //    sau 22h tính giờ xét value vào cột OT_102
        //    sau 5h tính giờ xét value vào cột OT_103

        //2.ngày nghỉ cuối tuần
        //    ca làm chính 08:00 - 17:00 và 20:00 - 05:00
        //    trước 22h tính giờ xét value vào cột OT_201
        //    sau 22h tính giờ xét value vào cột OT_202

        //3.ngày lễ
        //    ca làm chính 08:00 - 17:00 tính giờ xét value vào cột OT_301
        //    ca làm chính 17:00 - 05:00 tính giờ xét value vào cột OT_302
        public List<worksheet_detail> CalculateWorksheetDetail(List<worksheet_daily> rawData, List<(DateTime date, string note)> holidayList)
        {
            var result = new List<worksheet_detail>();

            var gio08h = new TimeSpan(08, 0, 0);
            var gio12h = new TimeSpan(12, 0, 0);
            var gio13h = new TimeSpan(13, 0, 0);
            var gio17h = new TimeSpan(17, 0, 0);
            var gio20h = new TimeSpan(20, 0, 0);
            var gio24h = new TimeSpan(23, 59, 59);
            var gio00h = new TimeSpan(00, 0, 0);
            var gio01h = new TimeSpan(01, 0, 0);
            var gio05h = new TimeSpan(05, 0, 0);

            foreach (var item in rawData)
            {
                if (item.DateTime_In == null || item.DateTime_Out == null)
                    continue;

                var inTime = item.DateTime_In.Value;
                var outTime = item.DateTime_Out.Value;
                var workDay = inTime.TimeOfDay < new TimeSpan(2, 0, 0) ? inTime.Date.AddDays(-1) : inTime.Date;
                var shift = (inTime.Hour >= 5 && inTime.Hour <= 16) ? "001" : "002";

                var detail = new worksheet_detail
                {
                    Work_Day = workDay,
                    Time_In = inTime,
                    Time_Out = outTime,
                    Shift_Code = shift,
                    Emp_Code = item.Emp_Code,
                    Factory = item.Factory,
                    Work_Hour = 0,
                    Lack_Hour = 0,
                    OT_101 = 0,
                    OT_102 = 0,
                    OT_103 = 0,
                    OT_201 = 0,
                    OT_202 = 0,
                    OT_301 = 0,
                    OT_302 = 0
                };
                detail.SetModified("J05468", true);

                #region Tính OT

                // Tìm ngày nghỉ trong danh sách holidayList (so sánh theo Date)
                string dayType = holidayList.FirstOrDefault(x => x.date.Date == workDay).note
                                 ?? ((workDay.DayOfWeek == DayOfWeek.Saturday || workDay.DayOfWeek == DayOfWeek.Sunday) ? "Weekend" : "Weekday");

                detail.note = dayType;

                // Helpers thời gian
                var t08 = workDay + gio08h;
                var t17 = workDay + gio17h;
                var t20 = workDay + gio20h;
                var t22 = workDay + new TimeSpan(22, 0, 0);
                var tMidnight = workDay.AddDays(1).Date;
                var tNext05 = tMidnight + gio05h;

                float CalcOT(DateTime start, DateTime end)
                {
                    // Điều chỉnh start nếu nằm trong khoảng trước các mốc chuẩn
                    if (start.Hour == 7 && start.Minute > 0)
                        start = start.Date + gio08h;   // 08:00

                    if (start.Hour == 19 && start.Minute > 0)
                        start = start.Date + gio20h;  // 20:00

                    // Điều chỉnh end nếu gần các mốc kết thúc chuẩn
                    if (end.Hour == 17 && end.Minute > 0)
                        end = end.Date + gio17h;      // 17:00

                    if (end.Hour == 5 && end.Minute > 0)
                        end = end.Date + gio05h;       // 05:00

                    // Tính giờ OT
                    var hours = (float)(end - start).TotalHours;
                    return hours >= 1 ? RoundDownToNearestHalf(hours) : 0;
                }

                var restStart = workDay + gio12h;
                var restEnd = workDay + gio13h;

                if (dayType == "Weekday")
                {
                    float restHour = 0f;

                    // --- OT_101: In trước 08:00 (Ca ngày)
                    if (inTime < t08)
                    {
                        detail.OT_101 += CalcOT(inTime, Min(outTime, t08));
                    }

                    // --- OT_101: Out từ 17:00 đến 22:00
                    if (outTime > t17 && inTime < t22)
                    {
                        detail.OT_101 += CalcOT(Max(inTime, t17), Min(outTime, t22));
                    }

                    // --- OT_102: Out sau 22:00 đến 05:00 sáng hôm sau
                    if (outTime > t22 && outTime <= tNext05)
                    {
                        var startOT = Max(inTime, t22);
                        var endOT = Min(outTime, tNext05);

                        // Trừ nghỉ đêm 00–01 nếu bao trùm
                        var restStartMid = workDay.AddDays(1).Date + gio00h;
                        var restEndMid = workDay.AddDays(1).Date + gio01h;
                        if (startOT < restStartMid && endOT > restEndMid)
                            restHour += 1f;

                        detail.OT_102 += CalcOT(startOT, endOT) - restHour;
                    }

                    // --- OT_101: Ca đêm đi trước 20:00
                    if (inTime >= t20 && inTime < t22)
                    {
                        var startOT = inTime;
                        var endOT = Min(outTime, t22);
                        detail.OT_101 += CalcOT(startOT, endOT);
                    }

                    // --- OT_103: Ca đêm out sau 05:00 sáng hôm sau
                    if (outTime > tNext05)
                    {
                        var startOT = Max(inTime, tNext05);
                        detail.OT_103 += CalcOT(startOT, outTime);
                    }

                    #region Tính giờ làm

                    bool onAM = (inTime.TimeOfDay < gio08h && outTime.TimeOfDay > gio12h)
                                || (inTime.TimeOfDay < gio01h && outTime.TimeOfDay > gio05h)
                                || (inTime.TimeOfDay > gio17h && inTime.TimeOfDay < gio24h && outTime.TimeOfDay > gio05h);
                    if (onAM)
                        detail.Work_Hour += 4;
                    else if (inTime.TimeOfDay > gio08h && inTime.TimeOfDay < gio12h && outTime.TimeOfDay > gio12h)
                    {
                        var timeAfter12h = (float)(gio12h - inTime.TimeOfDay).TotalHours;
                        detail.Work_Hour += timeAfter12h > 0 ? timeAfter12h : 5f + timeAfter12h;
                    }

                    bool onPM = (inTime.TimeOfDay < gio13h && outTime.TimeOfDay > gio17h)
                                || (inTime.TimeOfDay > gio17h && inTime.TimeOfDay < gio20h && outTime.TimeOfDay > gio00h);
                    if (onPM)
                        detail.Work_Hour += 4;
                    else if (inTime.TimeOfDay < gio13h && outTime.TimeOfDay < gio17h)
                    {
                        var timeAfter00h = (float)(outTime.TimeOfDay - gio13h).TotalHours;
                        detail.Work_Hour += timeAfter00h > 0 ? timeAfter00h : 0;
                    }
                    else if (inTime.TimeOfDay > gio17h && inTime.TimeOfDay < gio24h)
                    {
                        var timeAfter00h = (float)(gio24h - inTime.TimeOfDay).TotalHours;
                        detail.Work_Hour += timeAfter00h > 0 ? timeAfter00h : 24f + timeAfter00h;
                    }

                    detail.Work_Hour = RoundDownToInt((float)(detail.Work_Hour));
                    // Thiếu công nếu làm < 8h
                    detail.Lack_Hour = detail.Work_Hour < 8 ? (float)Math.Round(8f - detail.Work_Hour.Value, 2) : 0;

                    #endregion
                }
                else if (dayType == "Weekend") // logic khung giờ ngày nghỉ
                {

                    // OT_201: Từ inTime đến 22:00
                    if (inTime < t22)
                    {
                        var otStart = inTime;
                        var otEnd = Min(outTime, t22);

                        // Nếu thời gian làm chồng lên 12h–13h thì trừ 1h nghỉ trưa
                        float restHour = (otStart < restStart && otEnd > restEnd) ? 1.0f : 0f;
                        detail.OT_201 += CalcOT(otStart, otEnd) - restHour;
                    }

                    // Chỉ tính OT_202 nếu thực sự làm *sau 22:00*
                    if (inTime < tNext05 && outTime > t22)
                    {
                        var otStart = Max(inTime, t22);
                        var otEnd = outTime < tNext05 ? outTime : tNext05;

                        float restHour = 0f;

                        // Trừ 1h nếu chồng khung nghỉ trưa 12h–13h
                        if (otStart < restStart && otEnd > restEnd)
                            restHour += 1f;

                        // Trừ 1h nếu chồng khung 00h–01h
                        var restStartMid = workDay.AddDays(1).Date + gio00h;  // 00:00
                        var restEndMid = workDay.AddDays(1).Date + gio01h;    // 01:00
                        if (otStart < restStartMid && otEnd > restEndMid)
                            restHour += 1f;

                        detail.OT_202 += CalcOT(otStart, otEnd) - restHour;
                    }

                }
                else if (dayType == "Holiday")  // logic khung giờ ngày lễ
                {
                    // OT_201: Từ inTime đến 22:00
                    if (inTime < t22)
                    {
                        var otStart = inTime;
                        var otEnd = Min(outTime, t22);

                        // Nếu thời gian làm chồng lên 12h–13h thì trừ 1h nghỉ trưa
                        float restHour = (otStart < restStart && otEnd > restEnd) ? 1.0f : 0f;
                        detail.OT_301 += CalcOT(otStart, otEnd) - restHour;
                    }

                    // Chỉ tính OT_202 nếu thực sự làm *sau 22:00*
                    if (inTime < tNext05 && outTime > t22)
                    {
                        var otStart = Max(inTime, t22);
                        var otEnd = outTime < tNext05 ? outTime : tNext05;

                        float restHour = 0f;

                        // Trừ 1h nếu chồng khung nghỉ trưa 12h–13h
                        if (otStart < restStart && otEnd > restEnd)
                            restHour += 1f;

                        // Trừ 1h nếu chồng khung 00h–01h
                        var restStartMid = workDay.AddDays(1).Date + gio00h;  // 00:00
                        var restEndMid = workDay.AddDays(1).Date + gio01h;    // 01:00
                        if (otStart < restStartMid && otEnd > restEndMid)
                            restHour += 1f;

                        detail.OT_302 += CalcOT(otStart, otEnd) - restHour;
                    }
                }

                #endregion


                result.Add(detail);
            }
            result = result.OrderBy(c => c.Work_Hour).ToList();

            return result;

            // Helpers
            static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
            static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
        }

        public List<worksheet_detail> CalculateWorksheetDetail_v2(DateTime workDay, List<worksheet_daily> rawData, List<(DateTime date, string note)> holidayList)
        {
            try
            {
                var result = new List<worksheet_detail>();

                #region Khung giờ ca làm việc
                var gio08h = new TimeSpan(8, 0, 0);
                var gio12h = new TimeSpan(12, 0, 0);
                var gio13h = new TimeSpan(13, 0, 0);
                var gio17h = new TimeSpan(17, 0, 0);
                var gio20h = new TimeSpan(20, 0, 0);
                var gio22h = new TimeSpan(22, 0, 0);
                var gio24h = new TimeSpan(23, 59, 59);
                var gio00h = new TimeSpan(0, 0, 0);
                var gio01h = new TimeSpan(1, 0, 0);
                var gio05h = new TimeSpan(5, 0, 0);
                #endregion

                rawData = rawData.OrderByDescending(c => c.DateTime_In).ToList();

                var empCodes = new List<string> { "J04430" };
                rawData = rawData.Where(x => empCodes.Contains(x.Emp_Code)).ToList();

                foreach (var item in rawData)
                {
                    try
                    { 
                        string dayType = holidayList.FirstOrDefault(x => x.date.Date == workDay).note
                        ?? (workDay.DayOfWeek == DayOfWeek.Sunday
                        ? "Weekend"
                        : "Weekday");

                        workDay = workDay.Date;

                        if (item.DateTime_In == null || item.DateTime_Out == null)
                        {
                            // Kiểm tra xem đã tồn tại bản ghi có cùng Work_Day, Emp_Code, Factory chưa
                            bool isExist = result.Any(x =>
                                x.Work_Day == workDay.Date &&
                                x.Emp_Code == item.Emp_Code &&
                                x.Factory == item.Factory);

                            if (!isExist)
                            {
                                var add = new worksheet_detail
                                {
                                    Factory = item.Factory,
                                    Emp_Code = item.Emp_Code,
                                    Work_Day = workDay.Date,
                                    Time_In = item.DateTime_In,
                                    Time_Out = item.DateTime_Out,
                                    Shift_Code = (item.DateTime_In?.Hour >= 5 || item.DateTime_Out?.Hour < 22) ? "001" : "002",
                                    Day_Code = dayType,
                                    note = dayType
                                };
                                add.SetModified("J05468", false);
                                result.Add(add);
                            }

                            continue;
                        }

                        var inTime = item.DateTime_In.Value;
                        var outTime = item.DateTime_Out.Value;
                        var shift = (inTime.Hour >= 5 && inTime.Hour <= 16) ? "001" : "002";

                        var detail = new worksheet_detail
                        {
                            Factory = item.Factory,
                            Emp_Code = item.Emp_Code,
                            Work_Day = workDay,
                            Shift_Code = shift,
                            Day_Code = dayType,
                            Time_In = inTime,
                            Time_Out = outTime,
                            Work_Hour = 0,
                            Lack_Hour = 0,
                            OT_101 = 0,
                            OT_102 = 0,
                            OT_103 = 0,
                            OT_201 = 0,
                            OT_202 = 0,
                            OT_301 = 0,
                            OT_302 = 0,
                            note = dayType
                        };
                        detail.SetModified("J05468", false);

                        var t08 = workDay + gio08h;
                        var t17 = workDay + gio17h;
                        var t20 = workDay + gio20h;
                        var t22 = workDay + gio22h;
                        var tNext05 = workDay.AddDays(1).Date + gio05h;

                        float CalcOT(DateTime start, DateTime end)
                        {
                            if (start.Hour == 7 && start.Minute > 0) start = start.Date + gio08h;
                            if (start.Hour == 19 && start.Minute > 0) start = start.Date + gio20h;
                            if (end.Hour == 17 && end.Minute > 0) end = end.Date + gio17h;
                            if (end.Hour == 5 && end.Minute > 0) end = end.Date + gio05h;

                            // Làm tròn start lên
                            start = start.Minute > 30
                                    ? (start.Hour < 23
                                        ? new DateTime(start.Year, start.Month, start.Day, start.Hour + 1, 0, 0)
                                        : new DateTime(start.Year, start.Month, start.Day, 23, 59, 59).AddMinutes(1)) // hoặc AddHours(1)
                                    : new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute > 0 ? 30 : 0, 0);

                            // Làm tròn end xuống
                            end = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute > 30 ? 30 : 0, 0);

                            var hours = (float)(end - start).TotalHours;
                            return hours >= 1 ? RoundDownToNearestHalf(hours) : 0;
                        }

                        float restHour = 0;
                        var restStart = workDay + gio12h;
                        var restEnd = workDay + gio13h;
                        var restStartMid = workDay.AddDays(1) + gio00h;
                        var restEndMid = workDay.AddDays(1) + gio01h;

                        void SubtractRest(ref float ot, DateTime start, DateTime end)
                        {
                            float deduction = 0;
                            if (start < restStart && end > restEnd) deduction += 1f;
                            if (start < restStartMid && end > restEndMid) deduction += 1f;
                            ot -= deduction;
                        }

                        if (dayType == "Weekday")
                        {
                            #region Tính giờ làm (Work_Hour)

                            if ((inTime.TimeOfDay < gio08h && outTime.TimeOfDay > gio12h)
                                || (inTime.TimeOfDay < gio01h && outTime.TimeOfDay > gio05h)
                                || (inTime.TimeOfDay > gio17h && inTime.TimeOfDay < gio24h && outTime.TimeOfDay > gio05h))
                                detail.Work_Hour += 4;
                            else if (inTime.TimeOfDay > gio08h && inTime.TimeOfDay < gio12h && outTime.TimeOfDay > gio12h)
                                detail.Work_Hour += (float)(gio12h - inTime.TimeOfDay).TotalHours;

                            if ((inTime.TimeOfDay < gio13h && outTime.TimeOfDay > gio17h)
                                || (inTime.TimeOfDay > gio17h && inTime.TimeOfDay < gio20h && outTime.TimeOfDay > gio00h))
                                detail.Work_Hour += 4;
                            else if (inTime.TimeOfDay < gio13h && outTime.TimeOfDay > gio13h && outTime.TimeOfDay < gio17h)
                                detail.Work_Hour += (float)(outTime.TimeOfDay - gio13h).TotalHours;
                            else if (inTime.TimeOfDay > gio17h && inTime.TimeOfDay < gio24h)
                                detail.Work_Hour += (float)(gio24h - inTime.TimeOfDay).TotalHours;

                            if (detail.Work_Hour <= 0 || (outTime - inTime).TotalHours < 4)  // in out quá ngắn < 4h
                            {
                                if (outTime.TimeOfDay < gio12h)
                                    detail.Work_Hour = (float)(outTime.TimeOfDay - gio08h).TotalHours;
                                else if (outTime.TimeOfDay < gio24h)
                                    detail.Work_Hour = (float)(outTime.TimeOfDay - gio20h).TotalHours;
                            }

                            detail.Work_Hour = RoundDownToInt((float)(detail.Work_Hour));
                            detail.Lack_Hour = detail.Work_Hour < 8 ? (float)Math.Round(8f - detail.Work_Hour.Value, 2) : 0;

                            #endregion

                            #region Tính giờ OT
                            if (inTime < t08)
                                detail.OT_101 += CalcOT(inTime, Min(outTime, t08));
                            if (outTime > t17 && outTime < t22)
                                detail.OT_101 += CalcOT(Max(inTime, t17), Min(outTime, t22));
                            if (outTime > t22 && outTime <= tNext05)
                            {
                                detail.OT_101 += CalcOT(t17, t22);

                                var ot = CalcOT(Max(inTime, t22), Min(outTime, tNext05));
                                SubtractRest(ref ot, Max(inTime, t22), Min(outTime, tNext05));
                                detail.OT_102 += ot;
                            }

                            if (inTime > t17 && inTime < t20)
                                detail.OT_101 += CalcOT(inTime, Min(outTime, t20));
                            if (outTime > tNext05)
                                detail.OT_103 += CalcOT(Max(inTime, tNext05), outTime);
                            #endregion
                        }
                        else if (dayType == "Weekend")
                        {
                            if (inTime < t22)
                            {
                                var ot = CalcOT(inTime, Min(outTime, t22));
                                SubtractRest(ref ot, inTime, Min(outTime, t22));
                                detail.OT_201 += ot;
                            }

                            if (inTime < tNext05 && outTime > t22)
                            {
                                var ot = CalcOT(Max(inTime, t22), Max(outTime, tNext05));
                                SubtractRest(ref ot, Max(inTime, t22), Max(outTime, tNext05));
                                detail.OT_202 += ot;
                            }
                        }
                        else if (dayType == "Holiday")
                        {
                            if (inTime < t22)
                            {
                                var ot = CalcOT(inTime, Min(outTime, t22));
                                SubtractRest(ref ot, inTime, Min(outTime, t22));
                                detail.OT_301 += ot;
                            }

                            if (inTime < tNext05 && outTime > t22)
                            {
                                var ot = CalcOT(Max(inTime, t22), Max(outTime, tNext05));
                                SubtractRest(ref ot, Max(inTime, t22), Max(outTime, tNext05));
                                detail.OT_302 += ot;
                            }
                        }

                        result.Add(detail);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error in CalculateWorksheetDetail_v2: " + ex.Message);
                    }
                }

                return result;

                static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
                static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

            }
            catch (Exception ex)
            {
                throw new Exception("Error in CalculateWorksheetDetail_v2: " + ex.Message);
            }
        }

        #endregion
        #endregion
    }
}
