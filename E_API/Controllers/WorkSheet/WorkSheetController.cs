using E_Contract.Service;
using E_Model.Request.WorkSheet;
using E_Model.Table_SQL.WorkSheet;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.WorkSheet
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkSheetController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        public WorkSheetController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        [HttpPost("History")]
        public async Task<IActionResult> GetAttendanceHistory([FromBody] WorkSheetRequest request)
        {
            try
            {
                var listData = await _serviceWrapper.WorkSheet.SelectFilterAsync(request);

                return OK(listData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Export-excel")]
        public async Task<IActionResult> ExportWorkSheet([FromBody] WorkSheetRequest request)
        {
            try
            {
                var listData = (await _serviceWrapper.WorkSheet.SelectFilterAsync(request)).ToList();

                if (listData == null || !listData.Any())
                    return BadRequest("Không có dữ liệu để xuất Excel.");

                var columnMapping = new Dictionary<string, (string, string?)>
                {
                    { "Mã nhân viên", ("Emp_Code", null) },
                    { "Tên nhân viên", ("Emp_Name", null) },
                    { "Phòng ban", ("Dept_Name", null) },
                    { "Ngày công", ("Work_Day", "yyyy-MM-dd") },
                    { "Giờ vào", ("DateTime_In", null) },
                    { "Máy vào", ("Machine_In", null) },
                    { "Giờ ra", ("DateTime_Out", null) },
                    { "Máy ra", ("Machine_Out", null) },
                    { "Ca làm việc", ("Shift", null) },
                };


                // Tên file có đuôi .xlsx
                var fileName = $"WorkSheet_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Export và nhận về đường dẫn tương đối
                var relativePath = ExcelExtension.ExportToExcel(listData, columnMapping, "WorkSheet", fileName, "/WorkSheet");

                // Tạo đường dẫn đầy đủ cho client tải
                var fileUrl = $"{Request.Scheme}://{Request.Host}{relativePath}";

                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xuất Excel: {ex.Message}");
            }
        }

        [HttpPost("Compare-data")]
        public async Task<IActionResult> CompareData([FromQuery] DateTime? dateTime)
        {
            try
            {
                DateTime queryDate = dateTime ?? DateTime.Now;

                var listHRBarCode = (await _serviceWrapper.WorkSheet.SelectHR_BarcodeAsync(queryDate, "HRMConnection")).ToList();
                var listHR_WBarCode = (await _serviceWrapper.WorkSheet.SelectHR_BarcodeAsync(queryDate, "NEOEConnection")).ToList();

                if (listHRBarCode == null || !listHRBarCode.Any())
                    return BadRequest("Không có dữ liệu.");

                // Hàm tạo key từ 5 trường để so sánh
                string BuildKey(HR_BarCode x) =>
                    $"{x.CD_COMPANY?.Trim().ToUpper()}|{x.NO_CARD?.Trim().ToUpper()}|{x.DT_WORK?.Trim()}|{x.TM_CARD?.Trim().ToUpper()}|{x.CD_WCODE?.Trim().ToUpper()}";

                var setHRM = listHRBarCode.Select(BuildKey).ToHashSet();
                var setNEOE = listHR_WBarCode.Select(BuildKey).ToHashSet();

                var onlyInHRM = listHRBarCode
                    .Where(x => !setNEOE.Contains(BuildKey(x)))
                    .ToList();

                var onlyInNEOE = listHR_WBarCode
                    .Where(x => !setHRM.Contains(BuildKey(x)))
                    .ToList();

                return OK(new { onlyInNEOE, onlyInHRM });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xuất Excel: {ex.Message}");
            }
        }

        #region Tool-WorkSheet
        [HttpGet("TurnOn-Tool-00-09")]
        public async Task<IActionResult> ConvertWorkSheet_Time(DateTime? dateTime)
        {
            try
            {
                DateTime dayStart = (dateTime ?? DateTime.Now).Date;

                var from = dayStart.AddHours(0); // 00:00
                var to = dayStart.AddHours(9);   // 09:00

                var (success, message) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_00_09(from, to);
                var (success2, message2) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_to_HRBarcode(from, to);

                if (success || success2)
                    return Ok(new { success = true, message = message + $"\n Convert_WorkSheet_to_HRBarcode:" + message2 });
                else
                    return BadRequest(new { success = false, error = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }

        [HttpGet("TurnOn-Tool-09-12")]
        public async Task<IActionResult> ConvertWorkSheet_Time2(DateTime? dateTime)
        {
            try
            {
                DateTime dayStart = (dateTime ?? DateTime.Now).Date;

                var from = dayStart.AddHours(9); // 09:00
                var to = dayStart.AddHours(12);   // 12:00

                var (success, message) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_09_12(from, to);

                if (success)
                    return Ok(new { success = true, message = message });
                else
                    return BadRequest(new { success = false, error = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }

        [HttpGet("TurnOn-Tool-12-24")]
        public async Task<IActionResult> ConvertWorkSheet_Time3(DateTime? dateTime)
        {
            try
            {
                DateTime dayStart = (dateTime ?? DateTime.Now).Date;
                var from = dayStart.AddHours(12);  // 12:00 hôm nay
                var to = dayStart.AddDays(1);      // 00:00 sáng mai

                var (success, message) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_12_24(from, to);

                if (success)
                    return Ok(new { success = true, message = message });
                else
                    return BadRequest(new { success = false, error = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }


        #endregion
    }
}
