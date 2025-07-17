$(document).ready(function () {

    loadData();

});

const fakeData = [
    {
        type_name: "Màn hình",
        device_code: "MON-001",
        device_name: "Dell P2419H",
        quanlity: 2,
        device_config: "24\" FHD | IPS | 60Hz | Cổng: HDMI, DP, VGA",
        user_code: "NV001",
        user_name: "Nguyễn Văn A",
        status: "Đang sử dụng",
        note: "Màn hình chính phòng kế toán"
    },
    {
        type_name: "Case máy tính",
        device_code: "PC-001",
        device_name: "Case HP ProDesk 400 G6",
        quanlity: 1,
        device_config: "CPU: i5-10500 | RAM: 16GB | SSD: 512GB NVMe | PSU: 300W",
        user_code: "NV001",
        user_name: "Nguyễn Văn A",
        status: "Đang sử dụng",
        note: "Dùng cho nhân viên thiết kế"
    },
    {
        type_name: "Bàn phím",
        device_code: "KEY-001",
        device_name: "Logitech K120",
        quanlity: 1,
        device_config: "Full-size | USB | Chống tràn | Độ bền 10 triệu lần nhấn",
        user_code: "NV001",
        user_name: "Nguyễn Văn A",
        status: "Đang sử dụng",
        note: "Cấp phát cho nhân sự mới"
    },
    {
        type_name: "Chuột",
        device_code: "MOU-001",
        device_name: "Logitech M185",
        quanlity: 15,
        device_config: "Không dây | DPI: 1000 | Pin AA 12 tháng",
        user_code: "NV004",
        user_name: "Phạm Thị D",
        status: "Đang sử dụng",
        note: "Chuột đi kèm laptop"
    },
    {
        type_name: "Phần mềm",
        device_code: "SW-001",
        device_name: "Windows 11 Pro",
        quanlity: 30,
        device_config: "Bản quyền vĩnh viễn | OEM | 64-bit | Hỗ trợ TPM 2.0",
        user_code: "NV005",
        user_name: "Ngô Minh E",
        status: "Đang sử dụng",
        note: "Cài đặt cho các máy mới"
    },
    {
        type_name: "Phần mềm",
        device_code: "SW-002",
        device_name: "Microsoft Office 2021",
        quanlity: 25,
        device_config: "Gồm Word, Excel, PowerPoint | Bản quyền vĩnh viễn",
        user_code: "NV006",
        user_name: "Vũ Thị F",
        status: "Đang sử dụng",
        note: "Bản quyền theo máy"
    },
    {
        type_name: "Phần mềm",
        device_code: "SW-003",
        device_name: "AutoCAD 2023",
        quanlity: 8,
        device_config: "Bản quyền 1 năm | 64-bit | Cài đặt trên Windows 10/11",
        user_code: "NV007",
        user_name: "Đặng Văn G",
        status: "Đang sử dụng",
        note: "Dùng cho bộ phận kỹ thuật"
    },
    {
        type_name: "Tai nghe",
        device_code: "AUD-001",
        device_name: "Tai nghe Logitech H111",
        quanlity: 12,
        device_config: "Kết nối 3.5mm | Mic đàm thoại | Dùng cho PC/laptop",
        user_code: "NV008",
        user_name: "Lý Thị H",
        status: "Đang sử dụng",
        note: "Dùng cho bộ phận CSKH"
    },
    {
        type_name: "Ổ cứng",
        device_code: "HDD-001",
        device_name: "Seagate Barracuda 2TB",
        quanlity: 6,
        device_config: "3.5\" | SATA III | 7200RPM | Cache 256MB",
        user_code: "NV009",
        user_name: "Trương Văn I",
        status: "Đã thu hồi",
        note: "Chuyển sang kho lưu trữ"
    },
    {
        type_name: "USB",
        device_code: "USB-001",
        device_name: "Kingston 32GB",
        quanlity: 50,
        device_config: "USB 3.2 | Vỏ kim loại | Tốc độ đọc 100MB/s",
        user_code: "",
        user_name: "",
        status: "Chưa cấp phát",
        note: "Chờ phân bổ theo yêu cầu"
    }
];

const loadData = async () => {
    const $table = $('#device_table');

    // Nếu bảng đã được khởi tạo, thì hủy để tái tạo lại
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    const columns = [
        {
            data: null,
            orderable: false,
            render: (data, type, row) => `
                    <div class="form-check form-check-sm form-check-custom form-check-solid">
                        <input class="form-check-input" type="checkbox" value="">
                    </div>`
        },
        { data: 'type_name', title: 'Phân loại' },
        { data: 'device_code', title: 'Mã thiết bị' },
        { data: 'device_name', title: 'Tên thiết bị' },
        { data: 'quanlity', title: 'Số lượng' },
        { data: 'device_config', title: 'Cấu hình thiết bị' },
        {
            data: null,
            orderable: false,
            render: (data, type, row) => {
                if (!row.user_code) return `<span class="badge badge-light-secondary">Chưa gán</span>`;
                return `<div>
                            <span class="badge badge-light-info">${row.user_code}</span> 
                            <span>${row.user_name}</span>
                        </div>`;
            }
        },
        {
            data: 'status',
            title: 'Trạng thái',
            render: (data) => {
                let badgeClass = 'badge-light-secondary';
                if (data === 'Đang sử dụng') badgeClass = 'badge-light-success';
                else if (data === 'Đã thu hồi') badgeClass = 'badge-light-warning';
                else if (data === 'Hỏng') badgeClass = 'badge-light-danger';

                return `<span class="badge ${badgeClass} fw-bold">${data}</span>`;
            }
        },
        { data: 'note', title: 'Ghi chú' },
        {
            data: null,
            title: 'Chức năng',
            orderable: false,
            className: 'text-end',
            render: (_, __, row) => `
                <div class="dropdown">
                    <button class="btn btn-light btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        Lựa chọn
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item text-primary" href="#">Sửa</a></li>
                        <li><a class="dropdown-item text-danger" href="#" data-action="delete">Xóa</a></li>
                    </ul>
                </div>`
        }
    ];

    const options = {
        processing: true,
        serverSide: false,
        responsive: true,
        autoWidth: false,
        searching: true,
        paging: true,
        data: fakeData,
        columns: columns,
        language: {
            lengthMenu: "Hiển thị _MENU_ bản ghi mỗi trang",
            info: "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
            infoEmpty: "Không có dữ liệu",
            search: "Tìm kiếm:",
            zeroRecords: "Không tìm thấy kết quả phù hợp",
            paginate: {
                first: "Đầu",
                last: "Cuối",
                next: "Sau",
                previous: "Trước"
            },
            processing: "Đang xử lý..."
        }
    };

    $table.DataTable(options);
};


const exportClick = async () => {
    apiHelper.post(`${apiBase}/WorkSheet/Export-excel`,
        {
            dept_code: $('#cbo_Dept').val() || "",
            shift: $('#cbo_Shift').val() || "",
            from_date: $('#txt_FromDate').val() || null,
            to_date: $('#txt_ToDate').val() || null,
            textSearch: $('#txt_Search').val() || "",
            pageNumber: 1,
            pageSize: 0,
            isAscending: false,
        },
        function (res) {
            // Tạo liên kết tạm thời để tải xuống tệp Excel
            if (res && res.url) {
                const link = document.createElement('a');
                link.href = res.url;
                link.download = ''; // để trình duyệt tự lấy tên file
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            } else {
                alert("Không nhận được đường dẫn file.");
            }
        },
        function (err) {
            console.error('Lỗi export:', err);
        }),
        true,       // isAddToken
        false,      // isFormData
        true        // isBlob - cần truyền mới
}




