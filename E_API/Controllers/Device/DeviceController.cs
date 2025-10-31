using E_API.Filter;
using E_Contract.Service;
using E_Model.Request.Device;
using E_Model.Request.WorkSheet;
using E_Model.Response.Device;
using E_Model.Table_SQL.Device;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;

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

        #region Device Request
        [LoginAuthorize]
        [HttpPost("Select-Device-Request")]
        public async Task<IActionResult> GetDeviceRequest(DeviceRequest request)
        {
            try
            {
                // Lấy user_code từ JWT token để lọc dữ liệu theo quyền
                var userCodeClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var user_code = userCodeClaim?.Value ?? "";

                if (string.IsNullOrWhiteSpace(user_code))
                    return Unauthorized("Invalid or expired token. User code not found.");

                // Tạo dữ liệu mẫu dựa trên fake data từ frontend
                var deviceRequests = GenerateSampleDeviceRequests()
                    .Select(c => new DeviceRequestResponse
                    {
                        request_id = c.request_id,
                        request_date = c.request_date,
                        deadline = c.deadline,
                        requester_code = c.requester_code,
                        requester_name = c.requester_name,
                        requester_email = c.requester_email,
                        requester_position = c.requester_position,
                        requester_dept = c.requester_dept,
                        receive_dept = c.receive_dept,
                        title = c.title,
                        content = c.content,
                        status = c.status,
                        step = c.step
                    })
                    .ToList();


                if (request.status != "all")
                    deviceRequests = deviceRequests.Where(x => x.status == request.status).ToList();
                if (!string.IsNullOrEmpty(request.department))
                    deviceRequests = deviceRequests.Where(x => x.requester_dept == request.department).ToList();

                return OK(deviceRequests);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }
        
        [LoginAuthorize]
        [HttpGet("Select-Device-Request-Detail")]
        public async Task<IActionResult> GetDeviceRequestDetail(string? request_code)
        {
            try
            {
                // Tạo dữ liệu mẫu dựa trên fake data từ frontend
                var deviceRequests = GenerateSampleDeviceRequests();

                var filterDeviceRequests = deviceRequests.Where(x => x.request_id == request_code).ToList();

                return OK(filterDeviceRequests);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        private List<DeviceRequestDetailRespone> GenerateSampleDeviceRequests()
        {
            return new List<DeviceRequestDetailRespone>
            {
                // 1. Yêu cầu mới - trạng thái Request (chờ trưởng phòng duyệt)
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251022-001",
                    request_date = "2025/10/22",
                    deadline = "2025/10/23",
                    requester_code = "NV001",
                    requester_name = "Nguyễn Văn Anh",
                    requester_email = "nguyen.vana@company.vn",
                    requester_position = "Nhân viên IT",
                    requester_dept = "IT",
                    receive_dept = "IT",
                    title = "Yêu cầu cấp laptop mới cho nhân viên IT",
                    content = "Laptop cũ bị hỏng màn hình, cần thay thế để làm việc hiệu quả hơn",
                    status = "Request",
                    step = 1,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Warning",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>(),
                    attachments = new List<string>()
                },

                // 2. Yêu cầu cấp PC cho kế toán - trạng thái Approved (chờ IT xử lý)
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251021-002",
                    request_date = "2025/10/21",
                    deadline = "2025/10/22",
                    requester_code = "NV002",
                    requester_name = "Trần Thị Bình",
                    requester_email = "tran.thib@company.vn",
                    requester_position = "Kế toán viên",
                    requester_dept = "Kế toán",
                    receive_dept = "IT",
                    title = "Cần máy tính để bàn cho phòng kế toán",
                    content = "Phòng kế toán thiếu 1 máy tính để bàn cho nhân viên mới, cần gấp để xử lý báo cáo tháng",
                    status = "Approved",
                    step = 2,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV018",
                        approve_name = "Lê Văn Cường",
                        approve_position = "Trưởng phòng Kế toán",
                        approve_status = "Approved",
                        approve_date = "2025/10/21",
                        approve_content = "Đồng ý cấp máy tính cho nhân viên mới - cần gấp cho báo cáo"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 1,
                            device_code = "PC-001",
                            device_name = "PC Desktop Dell OptiPlex",
                            user_code = "NV002",
                            user_name = "Trần Thị Bình",
                            purpose = "Công việc kế toán và báo cáo tài chính"
                        }
                    },
                    attachments = new List<string> { "pc_specs.pdf", "invoice.pdf", "urgent_request.docx" }
                },

                // 3. Laptop cho kinh doanh - trạng thái Ongoing (IT đang xử lý)
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251020-003",
                    request_date = "2025/10/20",
                    deadline = "2025/10/21",
                    requester_code = "NV003",
                    requester_name = "Lê Văn Cường",
                    requester_email = "le.vanc@company.vn",
                    requester_position = "Nhân viên kinh doanh",
                    requester_dept = "Kinh doanh",
                    receive_dept = "IT",
                    title = "Yêu cầu cấp máy tính xách tay cho công tác kinh doanh",
                    content = "Cần laptop để đi gặp khách hàng, trình bày sản phẩm và làm việc từ xa",
                    status = "Ongoing",
                    step = 3,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV025",
                        approve_name = "Phạm Thị Dung",
                        approve_position = "Trưởng phòng Kinh doanh",
                        approve_status = "Approved",
                        approve_date = "2025/10/20",
                        approve_content = "Cần thiết cho việc gặp gỡ khách hàng và demo sản phẩm"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/20",
                        approve_content = "Đồng ý cấp laptop Lenovo ThinkPad X1 Carbon"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Ongoing",
                        confirm_date = "2025/10/20"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 2,
                            device_code = "LT-002",
                            device_name = "Laptop Lenovo ThinkPad X1 Carbon",
                            user_code = "NV003",
                            user_name = "Lê Văn Cường",
                            purpose = "Công tác kinh doanh và trình bày sản phẩm"
                        }
                    },
                    attachments = new List<string> { "laptop_specs.docx", "business_case.pdf" }
                },

                // 4. Màn hình cho nhân sự - trạng thái Confirm (đã bàn giao)
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251019-004",
                    request_date = "2025/10/19",
                    deadline = "2025/10/20",
                    requester_code = "NV004",
                    requester_name = "Phạm Thị Dung",
                    requester_email = "pham.thid@company.vn",
                    requester_position = "Trưởng phòng nhân sự",
                    requester_dept = "Nhân sự",
                    receive_dept = "IT",
                    title = "Cần thêm màn hình máy tính cho phòng nhân sự",
                    content = "Thiếu màn hình để làm việc hiệu quả hơn và phục vụ họp online với ứng viên",
                    status = "Confirm",
                    step = 4,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV032",
                        approve_name = "Hoàng Văn Em",
                        approve_position = "Trưởng phòng Nhân sự",
                        approve_status = "Approved",
                        approve_date = "2025/10/19",
                        approve_content = "Cần thiết cho việc phỏng vấn và họp online"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/19",
                        approve_content = "Cấp màn hình Dell UltraSharp 27 inch"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Confirm",
                        confirm_date = "2025/10/19"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 3,
                            device_code = "MN-003",
                            device_name = "Màn hình Dell UltraSharp 27 inch",
                            user_code = "NV004",
                            user_name = "Phạm Thị Dung",
                            purpose = "Làm việc nhân sự và họp online"
                        }
                    },
                    attachments = new List<string> { "monitor_specs.pdf", "setup_guide.pdf" }
                },

                // 5. Workstation cho kỹ thuật - trạng thái Done (hoàn tất)
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251018-005",
                    request_date = "2025/10/18",
                    deadline = "2025/10/19",
                    requester_code = "NV005",
                    requester_name = "Hoàng Văn Em",
                    requester_email = "hoang.vane@company.vn",
                    requester_position = "Kỹ thuật viên",
                    requester_dept = "Kỹ thuật",
                    receive_dept = "IT",
                    title = "Yêu cầu cấp máy tính chuyên dụng cho thiết kế",
                    content = "Cần máy tính có cấu hình cao để thiết kế CAD và làm việc với phần mềm kỹ thuật",
                    status = "Done",
                    step = 5,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV045",
                        approve_name = "Đỗ Thị Phương",
                        approve_position = "Trưởng phòng Kỹ thuật",
                        approve_status = "Approved",
                        approve_date = "2025/10/18",
                        approve_content = "Cần thiết cho công việc thiết kế kỹ thuật và CAD"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/18",
                        approve_content = "Cấp workstation HP Z4 cho phòng kỹ thuật"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Done",
                        confirm_date = "2025/10/18"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 4,
                            device_code = "WS-004",
                            device_name = "Workstation HP Z4",
                            user_code = "NV005",
                            user_name = "Hoàng Văn Em",
                            purpose = "Thiết kế kỹ thuật và CAD"
                        }
                    },
                    attachments = new List<string> { "workstation_specs.pdf", "cad_software_license.pdf" }
                },

                // 6. MacBook bị từ chối - trạng thái Rejected
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251017-006",
                    request_date = "2025/10/17",
                    deadline = "2025/10/18",
                    requester_code = "NV006",
                    requester_name = "Đỗ Thị Phương",
                    requester_email = "do.thif@company.vn",
                    requester_position = "Nhân viên marketing",
                    requester_dept = "Marketing",
                    receive_dept = "IT",
                    title = "Cần laptop để làm việc từ xa",
                    content = "Làm việc remote cần thiết bị di động, yêu cầu MacBook Pro 13 inch",
                    status = "Rejected",
                    step = 2,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV058",
                        approve_name = "Vũ Thị Hoa",
                        approve_position = "Trưởng phòng Marketing",
                        approve_status = "Rejected",
                        approve_date = "2025/10/17",
                        approve_content = "Không đồng ý vì công ty không hỗ trợ thiết bị Apple"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>(),
                    attachments = new List<string> { "laptop_request.pdf" },
                    reject_reason = "Công ty không hỗ trợ thiết bị Apple theo chính sách hiện hành"
                },

                // 7. Máy in cho hành chính - trạng thái Request (mới)
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251016-007",
                    request_date = "2025/10/16",
                    deadline = "2025/10/17",
                    requester_code = "NV007",
                    requester_name = "Vũ Thị Hoa",
                    requester_email = "vu.thih@company.vn",
                    requester_position = "Trưởng phòng Marketing",
                    requester_dept = "Marketing",
                    receive_dept = "IT",
                    title = "Cần cấp máy tính cho nhân viên marketing mới",
                    content = "Nhân viên mới cần máy tính để làm việc với các công cụ marketing và thiết kế",
                    status = "Request",
                    step = 1,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV058",
                        approve_name = "Vũ Thị Hoa",
                        approve_position = "Trưởng phòng Marketing",
                        approve_status = "Warning",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 5,
                            device_code = "LT-005",
                            device_name = "Laptop Dell Inspiron 15",
                            user_code = "NV007",
                            user_name = "Vũ Thị Hoa",
                            purpose = "Công việc marketing và thiết kế"
                        }
                    },
                    attachments = new List<string>()
                },

                // 8. Máy in cho hành chính - trạng thái Approved
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251015-008",
                    request_date = "2025/10/15",
                    deadline = "2025/10/16",
                    requester_code = "NV008",
                    requester_name = "Bùi Văn Nam",
                    requester_email = "bui.vann@company.vn",
                    requester_position = "Nhân viên hành chính",
                    requester_dept = "Hành chính",
                    receive_dept = "IT",
                    title = "Cần máy in cho phòng hành chính",
                    content = "Máy in cũ bị hỏng, cần thay thế để in ấn tài liệu và hợp đồng",
                    status = "Approved",
                    step = 2,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV071",
                        approve_name = "Đặng Thị Lan",
                        approve_position = "Trưởng phòng Hành chính",
                        approve_status = "Approved",
                        approve_date = "2025/10/15",
                        approve_content = "Máy in cần thiết cho công việc hành chính"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 6,
                            device_code = "PR-006",
                            device_name = "Máy in HP LaserJet Pro",
                            user_code = "NV008",
                            user_name = "Bùi Văn Nam",
                            purpose = "In ấn tài liệu hành chính và hợp đồng"
                        }
                    },
                    attachments = new List<string> { "printer_specs.pdf" }
                },

                // 9. Tablet cho sales - trạng thái Ongoing
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251014-009",
                    request_date = "2025/10/14",
                    deadline = "2025/10/15",
                    requester_code = "NV009",
                    requester_name = "Đặng Thị Lan",
                    requester_email = "dang.thil@company.vn",
                    requester_position = "Nhân viên bán hàng",
                    requester_dept = "Kinh doanh",
                    receive_dept = "IT",
                    title = "Cần tablet cho công tác bán hàng",
                    content = "Cần tablet để trình bày sản phẩm cho khách hàng và ghi nhận đơn hàng",
                    status = "Ongoing",
                    step = 3,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV025",
                        approve_name = "Phạm Thị Dung",
                        approve_position = "Trưởng phòng Kinh doanh",
                        approve_status = "Approved",
                        approve_date = "2025/10/14",
                        approve_content = "Cần thiết cho việc trình bày sản phẩm và chốt đơn hàng"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/14",
                        approve_content = "Cấp tablet Samsung Galaxy Tab S8"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Ongoing",
                        confirm_date = "2025/10/14"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 7,
                            device_code = "TB-007",
                            device_name = "Tablet Samsung Galaxy Tab S8",
                            user_code = "NV009",
                            user_name = "Đặng Thị Lan",
                            purpose = "Trình bày sản phẩm và chốt đơn hàng"
                        }
                    },
                    attachments = new List<string> { "tablet_specs.pdf", "business_case.pdf" }
                },

                // 10. Đa thiết bị cho phòng họp - trạng thái Confirm
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251013-010",
                    request_date = "2025/10/13",
                    deadline = "2025/10/14",
                    requester_code = "NV010",
                    requester_name = "Nguyễn Văn Thành",
                    requester_email = "nguyen.vanth@company.vn",
                    requester_position = "Trưởng phòng IT",
                    requester_dept = "IT",
                    receive_dept = "IT",
                    title = "Cần thiết bị cho phòng họp số 1",
                    content = "Phòng họp thiếu màn hình TV, webcam và loa để họp online hiệu quả",
                    status = "Confirm",
                    step = 4,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/13",
                        approve_content = "Cần thiết cho việc họp online và trình bày"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/13",
                        approve_content = "Tự phê duyệt thiết bị cho phòng họp"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Confirm",
                        confirm_date = "2025/10/13"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 8,
                            device_code = "TV-008",
                            device_name = "Smart TV Samsung 55 inch",
                            user_code = "MEETING001",
                            user_name = "Phòng họp số 1",
                            purpose = "Họp online và trình bày"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 9,
                            device_code = "WC-009",
                            device_name = "Webcam Logitech HD 1080p",
                            user_code = "MEETING001",
                            user_name = "Phòng họp số 1",
                            purpose = "Họp video call"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 10,
                            device_code = "SP-010",
                            device_name = "Loa Bluetooth JBL",
                            user_code = "MEETING001",
                            user_name = "Phòng họp số 1",
                            purpose = "Âm thanh họp online"
                        }
                    },
                    attachments = new List<string> { "meeting_equipment_specs.pdf", "room_layout.pdf" }
                },

                // 11. Smartphone cho quản lý - trạng thái Done
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251012-011",
                    request_date = "2025/10/12",
                    deadline = "2025/10/13",
                    requester_code = "NV011",
                    requester_name = "Nguyễn Văn Thành",
                    requester_email = "nguyen.vanth@company.vn",
                    requester_position = "Trưởng phòng IT",
                    requester_dept = "IT",
                    receive_dept = "IT",
                    title = "Cần smartphone cho công tác quản lý",
                    content = "Cần smartphone để quản lý từ xa và xử lý công việc khẩn cấp",
                    status = "Done",
                    step = 5,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV099",
                        approve_name = "Trần Văn Minh",
                        approve_position = "Giám đốc IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/12",
                        approve_content = "Cần thiết cho việc quản lý và xử lý công việc khẩn cấp"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/12",
                        approve_content = "Cấp iPhone 14 Pro cho trưởng phòng"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Done",
                        confirm_date = "2025/10/12"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 11,
                            device_code = "SP-011",
                            device_name = "iPhone 14 Pro 256GB",
                            user_code = "NV011",
                            user_name = "Nguyễn Văn Thành",
                            purpose = "Quản lý và xử lý công việc khẩn cấp"
                        }
                    },
                    attachments = new List<string> { "smartphone_specs.pdf", "management_policy.pdf" }
                },

                // 12. Máy scan cho kế toán - trạng thái Request
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251011-012",
                    request_date = "2025/10/11",
                    deadline = "2025/10/12",
                    requester_code = "NV012",
                    requester_name = "Lê Thị Mai",
                    requester_email = "le.thim@company.vn",
                    requester_position = "Kế toán trưởng",
                    requester_dept = "Kế toán",
                    receive_dept = "IT",
                    title = "Cần máy scan cho phòng kế toán",
                    content = "Cần máy scan để số hóa hóa đơn và chứng từ kế toán",
                    status = "Request",
                    step = 1,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV018",
                        approve_name = "Lê Văn Cường",
                        approve_position = "Trưởng phòng Kế toán",
                        approve_status = "Warning",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 12,
                            device_code = "SC-012",
                            device_name = "Máy scan Epson DS-570W",
                            user_code = "NV012",
                            user_name = "Lê Thị Mai",
                            purpose = "Số hóa hóa đơn và chứng từ kế toán"
                        }
                    },
                    attachments = new List<string> { "scanner_specs.pdf", "accounting_requirements.pdf" }
                },

                // 13. Laptop cho thiết kế - trạng thái Approved
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251010-013",
                    request_date = "2025/10/10",
                    deadline = "2025/10/11",
                    requester_code = "NV013",
                    requester_name = "Phạm Văn Hùng",
                    requester_email = "pham.vanh@company.vn",
                    requester_position = "Nhân viên thiết kế",
                    requester_dept = "Marketing",
                    receive_dept = "IT",
                    title = "Cần laptop cho công việc thiết kế đồ họa",
                    content = "Cần laptop cấu hình cao để chạy Adobe Creative Suite và thiết kế",
                    status = "Approved",
                    step = 2,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV058",
                        approve_name = "Vũ Thị Hoa",
                        approve_position = "Trưởng phòng Marketing",
                        approve_status = "Approved",
                        approve_date = "2025/10/10",
                        approve_content = "Cần thiết cho việc thiết kế và sáng tạo nội dung"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 13,
                            device_code = "LT-013",
                            device_name = "Laptop Dell XPS 15",
                            user_code = "NV013",
                            user_name = "Phạm Văn Hùng",
                            purpose = "Thiết kế đồ họa và Adobe Creative Suite"
                        }
                    },
                    attachments = new List<string> { "design_laptop_specs.pdf", "portfolio.pdf" }
                },

                // 14. Webcam cho làm việc từ xa - trạng thái Ongoing
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251009-014",
                    request_date = "2025/10/09",
                    deadline = "2025/10/10",
                    requester_code = "NV014",
                    requester_name = "Trần Văn Bình",
                    requester_email = "tran.vanb@company.vn",
                    requester_position = "Nhân viên phát triển",
                    requester_dept = "IT",
                    receive_dept = "IT",
                    title = "Cần webcam cho làm việc từ xa",
                    content = "Làm việc hybrid cần webcam chất lượng cao để họp online",
                    status = "Ongoing",
                    step = 3,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/09",
                        approve_content = "Cần thiết cho chế độ làm việc hybrid"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/09",
                        approve_content = "Cấp webcam Logitech BRIO 4K"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Ongoing",
                        confirm_date = "2025/10/09"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 14,
                            device_code = "WC-014",
                            device_name = "Webcam Logitech BRIO 4K",
                            user_code = "NV014",
                            user_name = "Trần Văn Bình",
                            purpose = "Họp online và làm việc từ xa"
                        }
                    },
                    attachments = new List<string> { "webcam_specs.pdf", "hybrid_work_policy.pdf" }
                },

                // 15. Server cho phòng IT - trạng thái Confirm
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251008-015",
                    request_date = "2025/10/08",
                    deadline = "2025/10/09",
                    requester_code = "NV015",
                    requester_name = "Lý Văn Đức",
                    requester_email = "ly.vand@company.vn",
                    requester_position = "Kỹ sư hệ thống",
                    requester_dept = "IT",
                    receive_dept = "IT",
                    title = "Cần server cho hệ thống backup",
                    content = "Cần server chuyên dụng để backup dữ liệu và đảm bảo an toàn thông tin",
                    status = "Confirm",
                    step = 4,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/08",
                        approve_content = "Cần thiết cho việc backup và bảo mật dữ liệu"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/08",
                        approve_content = "Cấp server Dell PowerEdge R750"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Confirm",
                        confirm_date = "2025/10/08"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 15,
                            device_code = "SV-015",
                            device_name = "Server Dell PowerEdge R750",
                            user_code = "IT_DEPT",
                            user_name = "Phòng IT",
                            purpose = "Backup dữ liệu và hệ thống"
                        }
                    },
                    attachments = new List<string> { "server_specs.pdf", "backup_policy.pdf", "security_requirements.pdf" }
                },

                // 16. Từ chối - yêu cầu không hợp lý
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251007-016",
                    request_date = "2025/10/07",
                    deadline = "2025/10/08",
                    requester_code = "NV016",
                    requester_name = "Ngô Thị Kim",
                    requester_email = "ngo.thik@company.vn",
                    requester_position = "Nhân viên hành chính",
                    requester_dept = "Hành chính",
                    receive_dept = "IT",
                    title = "Cần iPad Pro cho công việc văn phòng",
                    content = "Cần iPad Pro để làm việc và ghi chú, thay thế cho laptop",
                    status = "Rejected",
                    step = 2,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV071",
                        approve_name = "Đặng Thị Lan",
                        approve_position = "Trưởng phòng Hành chính",
                        approve_status = "Rejected",
                        approve_date = "2025/10/07",
                        approve_content = "Không cần thiết, có thể sử dụng laptop hiện có"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>(),
                    attachments = new List<string> { "ipad_request.pdf" },
                    reject_reason = "Thiết bị không cần thiết cho công việc hiện tại, có thể sử dụng laptop có sẵn"
                },

                // 17. Đa thiết bị cho team marketing - trạng thái Request
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251006-017",
                    request_date = "2025/10/06",
                    deadline = "2025/10/07",
                    requester_code = "NV017",
                    requester_name = "Vũ Thị Hoa",
                    requester_email = "vu.thih@company.vn",
                    requester_position = "Trưởng phòng Marketing",
                    requester_dept = "Marketing",
                    receive_dept = "IT",
                    title = "Cần thiết bị cho team marketing mới",
                    content = "Team marketing mở rộng cần 3 laptop và 2 tablet cho các chiến dịch mới",
                    status = "Request",
                    step = 1,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV058",
                        approve_name = "Vũ Thị Hoa",
                        approve_position = "Trưởng phòng Marketing",
                        approve_status = "Warning",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 16,
                            device_code = "LT-016",
                            device_name = "Laptop Dell Latitude 7420",
                            user_code = "NV017",
                            user_name = "Vũ Thị Hoa",
                            purpose = "Quản lý chiến dịch marketing"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 17,
                            device_code = "LT-017",
                            device_name = "Laptop Dell Latitude 7420",
                            user_code = "NV018",
                            user_name = "Team Marketing 1",
                            purpose = "Thực hiện chiến dịch marketing"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 18,
                            device_code = "LT-018",
                            device_name = "Laptop Dell Latitude 7420",
                            user_code = "NV019",
                            user_name = "Team Marketing 2",
                            purpose = "Phân tích dữ liệu marketing"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 19,
                            device_code = "TB-019",
                            device_name = "Tablet Apple iPad 10th Gen",
                            user_code = "NV020",
                            user_name = "Designer 1",
                            purpose = "Thiết kế và sáng tạo nội dung"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 20,
                            device_code = "TB-020",
                            device_name = "Tablet Apple iPad 10th Gen",
                            user_code = "NV021",
                            user_name = "Designer 2",
                            purpose = "Thiết kế UI/UX"
                        }
                    },
                    attachments = new List<string> { "team_equipment_request.pdf", "campaign_plan.pdf" }
                },

                // 18. Máy photocopy cho văn phòng - trạng thái Approved
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251005-018",
                    request_date = "2025/10/05",
                    deadline = "2025/10/06",
                    requester_code = "NV018",
                    requester_name = "Đặng Thị Lan",
                    requester_email = "dang.thil@company.vn",
                    requester_position = "Trưởng phòng Hành chính",
                    requester_dept = "Hành chính",
                    receive_dept = "IT",
                    title = "Cần máy photocopy cho văn phòng",
                    content = "Máy photocopy cũ hỏng, cần thay mới để phục vụ in ấn tài liệu công ty",
                    status = "Approved",
                    step = 2,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV071",
                        approve_name = "Đặng Thị Lan",
                        approve_position = "Trưởng phòng Hành chính",
                        approve_status = "Approved",
                        approve_date = "2025/10/05",
                        approve_content = "Cần thiết cho việc in ấn tài liệu công ty"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "",
                        approve_name = "",
                        approve_position = "",
                        approve_status = "",
                        approve_date = "",
                        approve_content = ""
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "",
                        confirm_name = "",
                        confirm_position = "",
                        confirm_status = "",
                        confirm_date = ""
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 21,
                            device_code = "PC-021",
                            device_name = "Máy photocopy Canon imageRUNNER",
                            user_code = "OFFICE",
                            user_name = "Văn phòng công ty",
                            purpose = "In ấn và photocopy tài liệu"
                        }
                    },
                    attachments = new List<string> { "photocopy_specs.pdf", "office_needs.pdf" }
                },

                // 19. Dự án đặc biệt - trạng thái Ongoing
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251004-019",
                    request_date = "2025/10/04",
                    deadline = "2025/10/05",
                    requester_code = "NV019",
                    requester_name = "Bùi Văn Tùng",
                    requester_email = "bui.vant@company.vn",
                    requester_position = "Project Manager",
                    requester_dept = "Kinh doanh",
                    receive_dept = "IT",
                    title = "Thiết bị cho dự án đặc biệt Q4/2025",
                    content = "Dự án Q4 cần thiết bị đặc thù để demo sản phẩm cho đối tác lớn",
                    status = "Ongoing",
                    step = 3,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV099",
                        approve_name = "Trần Văn Minh",
                        approve_position = "Giám đốc Kinh doanh",
                        approve_status = "Approved",
                        approve_date = "2025/10/04",
                        approve_content = "Ưu tiên cao cho dự án Q4 với đối tác chiến lược"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/04",
                        approve_content = "Ưu tiên cấp thiết bị cho dự án quan trọng"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Ongoing",
                        confirm_date = "2025/10/04"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 22,
                            device_code = "LT-022",
                            device_name = "Laptop Gaming ASUS ROG",
                            user_code = "NV019",
                            user_name = "Bùi Văn Tùng",
                            purpose = "Demo sản phẩm game cho đối tác"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 23,
                            device_code = "VR-023",
                            device_name = "Kính VR Oculus Quest 2",
                            user_code = "NV019",
                            user_name = "Bùi Văn Tùng",
                            purpose = "Trải nghiệm sản phẩm VR"
                        },
                        new EquipmentInfo
                        {
                            serial_id = 24,
                            device_code = "TB-024",
                            device_name = "Tablet Microsoft Surface Pro",
                            user_code = "NV019",
                            user_name = "Bùi Văn Tùng",
                            purpose = "Trình bày proposal và báo cáo"
                        }
                    },
                    attachments = new List<string> { "project_specs.pdf", "partner_agreement.pdf", "demo_requirements.pdf" }
                },

                // 20. Nâng cấp thiết bị cũ - trạng thái Done
                new DeviceRequestDetailRespone
                {
                    request_id = "REQ-20251003-020",
                    request_date = "2025/10/03",
                    deadline = "2025/10/04",
                    requester_code = "NV020",
                    requester_name = "Lê Văn Hùng",
                    requester_email = "le.vanh@company.vn",
                    requester_position = "Kỹ sư phần mềm",
                    requester_dept = "IT",
                    receive_dept = "IT",
                    title = "Nâng cấp RAM cho máy tính làm việc",
                    content = "Máy tính hiện tại RAM chỉ 8GB, không đủ cho việc chạy nhiều ứng dụng",
                    status = "Done",
                    step = 5,
                    request_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/03",
                        approve_content = "Đồng ý nâng cấp RAM để tăng hiệu suất làm việc"
                    },
                    publish_approve = new ApprovalInfo
                    {
                        approve_code = "NV011",
                        approve_name = "Nguyễn Văn Thành",
                        approve_position = "Trưởng phòng IT",
                        approve_status = "Approved",
                        approve_date = "2025/10/03",
                        approve_content = "Nâng cấp RAM từ 8GB lên 32GB"
                    },
                    publish_confirm = new ConfirmationInfo
                    {
                        confirm_code = "NV066",
                        confirm_name = "Nguyễn Văn Khoa",
                        confirm_position = "Nhân viên IT",
                        confirm_status = "Done",
                        confirm_date = "2025/10/03"
                    },
                    equipment = new List<EquipmentInfo>
                    {
                        new EquipmentInfo
                        {
                            serial_id = 25,
                            device_code = "RAM-025",
                            device_name = "RAM DDR4 32GB (2x16GB)",
                            user_code = "NV020",
                            user_name = "Lê Văn Hùng",
                            purpose = "Nâng cấp hiệu suất máy tính"
                        }
                    },
                    attachments = new List<string> { "ram_upgrade_specs.pdf", "performance_report.pdf" }
                }
            };
        }

        #endregion

        #region Device Management
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
        #endregion
    }
}
