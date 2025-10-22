using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.Device
{
    public class DeviceRequestDetailRespone
    {
        public string? request_id { get; set; }
        public string? request_date { get; set; }
        public string? deadline { get; set; }
        public string? requester_code { get; set; }
        public string? requester_name { get; set; }
        public string? requester_email { get; set; }
        public string? requester_position { get; set; }
        public string? requester_dept { get; set; }
        public string? receive_dept { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? status { get; set; }
        public int? step { get; set; }

        // Thông tin phê duyệt cấp 1 (trưởng phòng yêu cầu)
        public ApprovalInfo request_approve { get; set; } = new ApprovalInfo();

        // Thông tin phê duyệt cấp 2 (trưởng phòng IT)
        public ApprovalInfo publish_approve { get; set; } = new ApprovalInfo();

        // Thông tin xác nhận hoàn thành (nhân viên IT)
        public ConfirmationInfo publish_confirm { get; set; } = new ConfirmationInfo();

        // Danh sách thiết bị yêu cầu
        public List<EquipmentInfo> equipment { get; set; } = new List<EquipmentInfo>();

        // Danh sách file đính kèm
        public List<string> attachments { get; set; } = new List<string>();

        // Lý do từ chối (nếu có)
        public string? reject_reason { get; set; }
    }

    public class ApprovalInfo
    {
        public string? approve_code { get; set; }
        public string? approve_name { get; set; }
        public string? approve_position { get; set; }
        public string? approve_status { get; set; }
        public string? approve_date { get; set; }
        public string? approve_content { get; set; }
    }

    public class ConfirmationInfo
    {
        public string? confirm_code { get; set; }
        public string? confirm_name { get; set; }
        public string? confirm_position { get; set; }
        public string? confirm_status { get; set; }
        public string? confirm_date { get; set; }
    }

    public class EquipmentInfo
    {
        public int? serial_id { get; set; }
        public string? device_code { get; set; }
        public string? device_name { get; set; }
        public string? user_code { get; set; }
        public string? user_name { get; set; }
        public string? purpose { get; set; }
    }
}
