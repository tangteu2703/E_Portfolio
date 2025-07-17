$(document).ready(function () {

    loadTransferHistory();

});

const transferHistorySimplified = [
    {
        device_code: "PC-001",
        device_name: "HP ProDesk 400 G6",
        user_code: "",
        user_name: "",
        from_date: "2024-03-01",
        to_date: "2024-08-01",
        status: "",
        note: "Nhập kho"
    },
    {
        device_code: "PC-001",
        device_name: "HP ProDesk 400 G6",
        user_code: "NV001",
        user_name: "Nguyễn Văn A",
        from_date: "2024-03-01",
        to_date: "2024-08-14",
        status: "Còn tốt",
        note: "Sử dụng tại văn phòng tầng 2"
    },
    {
        device_code: "PC-001",
        device_name: "HP ProDesk 400 G6",
        user_code: "NV002",
        user_name: "Trần Thị B",
        from_date: "2024-08-15",
        to_date: null,
        status: "Đang sử dụng",
        note: "Chuyển từ NV001"
    },
    {
        device_code: "MON-001",
        device_name: "Dell P2419H",
        user_code: "NV003",
        user_name: "Lê Văn C",
        from_date: "2024-05-10",
        to_date: "2025-01-04",
        status: "",
        note: "Màn hình chính"
    },
    {
        device_code: "MON-001",
        device_name: "Dell P2419H",
        user_code: "NV004",
        user_name: "Phạm Thị D",
        from_date: "2025-01-05",
        to_date: null,
        status: "Đang sử dụng",
        note: "Thay đổi vị trí"
    },
    {
        device_code: "KEY-001",
        device_name: "Logitech K120",
        user_code: "NV005",
        user_name: "Ngô Minh E",
        from_date: "2023-11-12",
        to_date: "2024-09-30",
        status: "Còn tốt",
        note: "Bàn phím cấp phát ban đầu"
    },
    {
        device_code: "KEY-001",
        device_name: "Logitech K120",
        user_code: "NV006",
        user_name: "Vũ Thị F",
        from_date: "2024-10-01",
        to_date: null,
        status: "Đang sử dụng",
        note: "Sử dụng lại"
    },
    {
        device_code: "SW-003",
        device_name: "AutoCAD 2023",
        user_code: "",
        user_name: "",
        from_date: "2024-09-05",
        to_date: "2025-06-19",
        status: "",
        note: "Thiết kế kết cấu"
    },
    {
        device_code: "SW-003",
        device_name: "AutoCAD 2023",
        user_code: "NV008",
        user_name: "Lý Thị H",
        from_date: "2025-06-20",
        to_date: null,
        status: "Đang sử dụng",
        note: "Chuyển dự án"
    },
    {
        device_code: "USB-001",
        device_name: "Kingston 32GB",
        user_code: "",
        user_name: "",
        from_date: "2025-03-14",
        to_date: "2025-06-30",
        status: "",
        note: "Phục vụ tạm thời"
    },
    {
        device_code: "USB-001",
        device_name: "Kingston 32GB",
        user_code: "NV010",
        user_name: "Đỗ Thị J",
        from_date: "2025-07-01",
        to_date: null,
        status: "Đang sử dụng",
        note: "Hiệu năng thấp"
    },
    {
        device_code: "MOU-001",
        device_name: "Logitech M185",
        user_code: "NV011",
        user_name: "Nguyễn Thanh K",
        from_date: "2024-12-01",
        to_date: null,
        status: "Hỏng",
        note: "Đã báo lỗi"
    },
    {
        device_code: "AUD-001",
        device_name: "Tai nghe H111",
        user_code: "NV012",
        user_name: "Phạm Minh L",
        from_date: "2025-01-10",
        to_date: null,
        status: "Đang sử dụng",
        note: "Dùng để họp online"
    },
    {
        device_code: "HDD-001",
        device_name: "Seagate 2TB",
        user_code: "NV013",
        user_name: "Trần Văn M",
        from_date: "2024-07-20",
        to_date: "2025-06-01",
        status: "Còn tốt",
        note: "Sao lưu dự phòng"
    },
    {
        device_code: "HDD-001",
        device_name: "Seagate 2TB",
        user_code: "NV014",
        user_name: "Vũ Thị N",
        from_date: "2025-06-02",
        to_date: null,
        status: "Đang sử dụng",
        note: "Backup hàng tuần"
    },
    {
        device_code: "SW-002",
        device_name: "Microsoft Office 2021",
        user_code: "NV015",
        user_name: "Nguyễn Quốc O",
        from_date: "2024-04-01",
        to_date: null,
        status: "Đang sử dụng",
        note: "Cài đặt trên máy mới"
    },
    {
        device_code: "SW-001",
        device_name: "Windows 11 Pro",
        user_code: "NV016",
        user_name: "Nguyễn Thị P",
        from_date: "2023-10-10",
        to_date: null,
        status: "Đang sử dụng",
        note: "Cấp theo máy HP"
    },
    {
        device_code: "MON-002",
        device_name: "Samsung LF24T350",
        user_code: "NV017",
        user_name: "Trần Quốc Q",
        from_date: "2025-02-05",
        to_date: null,
        status: "Đang sử dụng",
        note: "Văn phòng hành chính"
    },
    {
        device_code: "PC-002",
        device_name: "Lenovo ThinkCentre M70",
        user_code: "NV018",
        user_name: "Lê Thị R",
        from_date: "2025-04-18",
        to_date: null,
        status: "",
        note: "Máy mới cấp"
    },
    {
        device_code: "KEY-002",
        device_name: "Razer Cynosa Lite",
        user_code: "NV019",
        user_name: "Hoàng Văn S",
        from_date: "2025-05-01",
        to_date: null,
        status: "Đang sử dụng",
        note: "Đề nghị kiểm tra"
    },
    {
        device_code: "MOU-002",
        device_name: "Rapoo M160 Silent",
        user_code: "NV020",
        user_name: "Lý Thị T",
        from_date: "2024-11-20",
        to_date: null,
        status: "Đang sử dụng",
        note: "Không tiếng ồn"
    }
];

const loadTransferHistory = async () => {
    const $table = $('#history_table');

    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    const columns = [
        {
            data: null,
            orderable: false,
            className: "text-center",
            width: "5%",
            render: () => `
                <div class="form-check form-check-sm form-check-custom form-check-solid">
                    <input class="form-check-input" type="checkbox" value="">
                </div>`
        },
        { data: 'device_code', title: 'Mã thiết bị' },
        { data: 'device_name', title: 'Tên thiết bị' },
        {
            data: null,
            title: 'Người sử dụng',
            render: (row) => `${row.user_code ?? ''} - ${row.user_name ?? ''}`
        },
        {
            data: 'from_date',
            title: 'Từ ngày',
            render: (data) => data ? moment(data).format("YYYY-MM-DD") : ''
        },
        {
            data: 'to_date',
            title: 'Đến ngày',
            render: (data) => data ? moment(data).format("YYYY-MM-DD") : '<span class="text-muted">Hiện tại</span>'
        },
        {
            data: 'status',
            title: 'Tình trạng',
            render: (data) => {
                let color = 'secondary';
                if (data.includes('Mới')) color = 'secondary';
                else if (data.includes('tốt') || data.includes('sử dụng')) color = 'success';
                else if (data.includes('hỏng') || data.includes('lỗi')) color = 'danger';
                else if (data.includes('thu hồi')) color = 'warning';

                return `<span class="badge badge-light-${color} fw-semibold">${data}</span>`;
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
        data: transferHistorySimplified, // dữ liệu đơn giản hóa
        columns,
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




