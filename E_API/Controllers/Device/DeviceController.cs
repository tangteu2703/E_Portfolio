using E_Contract.Service;
using E_Model.Request.Device;
using E_Model.Request.WorkSheet;
using E_Model.Response.Device;
using E_Model.Table_SQL.Device;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.Device
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        public DeviceController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        [HttpPost("Management")]
        public async Task<IActionResult> GetDataSetup([FromBody] DeviceRequest request)
        {
            try
            {
                var result = await _serviceWrapper.DeviceManagement.SelectFilterAsync(request);

                // Trả kết quả
                return Ok(new
                {
                    data = result.listData,
                    recordsTotal = result.recordsTotal,
                    recordsFiltered = result.recordsFiltered
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Import-excel")]
        public async Task<IActionResult> ImportExcel([FromForm] ImportExcelRequest request)
        {
            try
            {
                var columnMapping = new Dictionary<string, string>
                {
                    { "Phân loại", "type_name" },
                    { "Mã thiết bị", "device_code" },
                    { "Tên thiết bị", "device_name" },
                    { "Cấu hình thiết bị", "device_config" },
                    { "Số lượng", "quantity" },
                    { "Trạng thái", "status_name" },
                    { "Mã nhân viên", "user_code" },
                    { "Tên nhân viên", "full_name" },
                    { "Ghi chú", "note" }
                };

                using var stream = request.file.OpenReadStream();
                var list = ExcelExtension.ImportFromExcel<DeviceManagementRespone>(stream, columnMapping);
                return OK(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi import Excel: {ex.Message}");
            }
        }


        [HttpPost("Export-excel")]
        public async Task<IActionResult> ExportExcel([FromBody] DeviceRequest request)
        {
            try
            {
                var listData = (await _serviceWrapper.DeviceManagement.SelectFilterAsync(request)).listData.ToList();

                if (listData == null || !listData.Any())
                    return BadRequest("Không có dữ liệu để xuất Excel.");

                var columnMapping = new Dictionary<string, (string, string?)>
                {
                    { "Phân loại", ("type_name", null) },
                    { "Mã thiết bị", ("device_code", null) },
                    { "Tên thiết bị", ("device_name", null) },
                    { "Cấu hình thiết bị", ("device_config", null) },
                    { "Số lượng", ("quantity", null) },
                    { "Trạng thái", ("status_name", null) },
                    { "Mã nhân viên", ("user_code", null) },
                    { "Tên nhân viên", ("full_name", null) },
                    { "Ghi chú", ("note", null) },
                };


                // Tên file có đuôi .xlsx
                var fileName = $"DeviceManagement_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Export và nhận về đường dẫn tương đối
                var relativePath = ExcelExtension.ExportToExcel(listData, columnMapping, "DeviceManagement", fileName, "/Device/DeviceManagement");

                // Tạo đường dẫn đầy đủ cho client tải
                var fileUrl = $"{Request.Scheme}://{Request.Host}{relativePath}";

                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xuất Excel: {ex.Message}");
            }
        }
    }
}
