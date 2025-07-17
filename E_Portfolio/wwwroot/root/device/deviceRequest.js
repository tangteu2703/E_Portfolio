$(document).ready(function () {

    loadDeviceRequestTable();

});

const deviceRequestData = [
    {
        request_id: "REQ-001",
        request_date: "2025-07-10",
        deadline: "2025-07-15",
        requester_code: "NV001",
        requester_name: "Nguyễn Văn A",
        approver_name: "Trần Thị B",
        device_code: "MON-001",
        device_name: "Màn hình Dell P2419H",
        quantity: 1,
        reason: "Làm việc thiết kế",
        status: "Chờ duyệt"
    },
    {
        request_id: "REQ-002",
        request_date: "2025-07-09",
        deadline: "2025-07-12",
        requester_code: "NV002",
        requester_name: "Lê Văn C",
        approver_name: "Ngô Minh D",
        device_code: "KEY-001",
        device_name: "Bàn phím Logitech K120",
        quantity: 2,
        reason: "Thay thế bàn phím cũ hỏng",
        status: "Đã duyệt"
    },
    {
        request_id: "REQ-003",
        request_date: "2025-07-08",
        deadline: "2025-07-09",
        requester_code: "NV003",
        requester_name: "Phạm Thị E",
        approver_name: "Trần Thị B",
        device_code: "MOU-001",
        device_name: "Chuột Logitech M185",
        quantity: 1,
        reason: "Phục vụ làm việc",
        status: "Đã bàn giao"
    },
    {
        request_id: "REQ-004",
        request_date: "2025-07-06",
        deadline: "2025-07-10",
        requester_code: "NV004",
        requester_name: "Nguyễn Minh F",
        approver_name: "Vũ Quốc G",
        device_code: "AUD-001",
        device_name: "Tai nghe H111",
        quantity: 1,
        reason: "Họp online",
        status: "Đã bàn giao"
    },
    {
        request_id: "REQ-005",
        request_date: "2025-07-05",
        deadline: "2025-07-07",
        requester_code: "NV005",
        requester_name: "Hoàng Thị H",
        approver_name: "Ngô Minh D",
        device_code: "SW-003",
        device_name: "AutoCAD 2023",
        quantity: 1,
        reason: "Làm kỹ thuật",
        status: "Từ chối"
    },
    {
        request_id: "REQ-006",
        request_date: "2025-07-04",
        deadline: "2025-07-06",
        requester_code: "NV006",
        requester_name: "Lý Văn I",
        approver_name: "Trần Thị B",
        device_code: "USB-001",
        device_name: "USB Kingston 32GB",
        quantity: 3,
        reason: "Chuyển tài liệu",
        status: "Đã duyệt"
    },
    {
        request_id: "REQ-007",
        request_date: "2025-07-03",
        deadline: "2025-07-10",
        requester_code: "NV007",
        requester_name: "Đặng Thị K",
        approver_name: "Trương Minh L",
        device_code: "PC-001",
        device_name: "Case HP ProDesk 400",
        quantity: 1,
        reason: "Máy cũ chậm",
        status: "Đã bàn giao"
    },
    {
        request_id: "REQ-008",
        request_date: "2025-07-02",
        deadline: "2025-07-05",
        requester_code: "NV008",
        requester_name: "Võ Quốc M",
        approver_name: "Trần Thị B",
        device_code: "SW-001",
        device_name: "Windows 11 Pro",
        quantity: 1,
        reason: "Máy mới chưa có OS",
        status: "Chờ duyệt"
    },
    {
        request_id: "REQ-009",
        request_date: "2025-07-01",
        deadline: "2025-07-08",
        requester_code: "NV009",
        requester_name: "Nguyễn Hải N",
        approver_name: "Vũ Quốc G",
        device_code: "HDD-001",
        device_name: "Ổ cứng Seagate 2TB",
        quantity: 1,
        reason: "Backup dữ liệu",
        status: "Đã duyệt"
    },
    {
        request_id: "REQ-010",
        request_date: "2025-06-30",
        deadline: "2025-07-02",
        requester_code: "NV010",
        requester_name: "Phạm Thị O",
        approver_name: "Trần Thị B",
        device_code: "AUD-002",
        device_name: "Tai nghe Logitech H111",
        quantity: 1,
        reason: "Chuyển bộ phận",
        status: "Từ chối"
    }
];

const loadDeviceRequestTable = () => {
    const $table = $('#request_table');

    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        data: deviceRequestData,
        responsive: true,
        autoWidth: false,
        searching: true,
        paging: true,
        columns: [
            { data: 'request_id', title: 'Mã yêu cầu' },
            {
                data: 'request_date',
                title: 'Ngày yêu cầu',
                render: d => moment(d).format("YYYY-MM-DD")
            },
            {
                data: 'deadline',
                title: 'Hạn bàn giao',
                render: d => moment(d).format("YYYY-MM-DD")
            },
            {
                data: null,
                title: 'Người yêu cầu',
                render: r => `${r.requester_code} - ${r.requester_name}`
            },
            { data: 'approver_name', title: 'Người phê duyệt' },
            {
                data: null,
                title: 'Thiết bị',
                render: r => `${r.device_code} - ${r.device_name}`
            },
            { data: 'quantity', title: 'Số lượng' },
            { data: 'reason', title: 'Lý do' },
            {
                data: 'status',
                title: 'Trạng thái',
                render: (status) => {
                    let cls = "secondary";
                    if (status === "Chờ duyệt") cls = "warning";
                    else if (status === "Đã duyệt") cls = "info";
                    else if (status === "Đã bàn giao") cls = "success";
                    else if (status === "Từ chối") cls = "danger";
                    return `<span class="badge badge-light-${cls}">${status}</span>`;
                }
            },
            {
                data: null,
                orderable: false,
                className: 'text-end',
                render: () => `
                    <div class="dropdown">
                        <button class="btn btn-light btn-sm dropdown-toggle" data-bs-toggle="dropdown">
                            Thao tác
                        </button>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item" href="#">Chi tiết</a></li>
                            <li><a class="dropdown-item text-danger" href="#">Xóa</a></li>
                        </ul>
                    </div>`
            }
        ],
        language: {
            lengthMenu: "Hiển thị _MENU_ bản ghi",
            search: "Tìm kiếm:",
            info: "Hiển thị _START_ đến _END_ trong _TOTAL_ bản ghi",
            zeroRecords: "Không có dữ liệu",
            paginate: {
                first: "Đầu",
                last: "Cuối",
                next: "Sau",
                previous: "Trước"
            }
        }
    });
};





