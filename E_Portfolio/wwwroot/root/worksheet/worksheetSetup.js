$(document).ready(function () {

    loadData();

});

const fakeData = [
    {
        type_code: "Ca ngày",
        start_time: "08:00:00",
        start_break: "12:00:00",
        end_break: "13:00:00",
        end_time: "17:00:00",
        note: "Làm việc ban ngày"
    },
    {
        type_code: "Ca đêm",
        start_time: "20:00:00",
        start_break: "00:00:00",
        end_break: "01:00:00",
        end_time: "05:00:00",
        note: "Làm việc ban đêm"
    },
    {
        type_code: "Kíp A",
        start_time: "06:00:00",
        start_break: "12:00:00",
        end_break: "13:00:00",
        end_time: "18:00:00",
        note: "Ca 12 tiếng – kíp A"
    },
    {
        type_code: "Kíp B",
        start_time: "18:00:00",
        start_break: "00:00:00",
        end_break: "01:00:00",
        end_time: "06:00:00",
        note: "Ca 12 tiếng – kíp B"
    },
    {
        type_code: "Nghỉ lễ",
        start_time: "2025-01-01 00:00:00",
        end_time: "2025-01-01 23:59:59",
        start_break: null,
        end_break: null,
        note: "01/01 - Tết Dương Lịch"
    },
    {
        type_code: "Nghỉ lễ",
        start_time: "2025-01-28 00:00:00", 
        end_time: "2025-02-03 23:59:59",  
        start_break: null,
        end_break: null,
        note: "Tết Nguyên Đán 2025 (29 AL -> mùng 5 AL)"
    },
    {
        type_code: "Nghỉ lễ",
        start_time: "2025-04-10 00:00:00",
        end_time: "2025-04-10 23:59:59",
        start_break: null,
        end_break: null,
        note: "Giỗ Tổ Hùng Vương - 10/03 âm lịch"
    },
    {
        type_code: "Nghỉ lễ",
        start_time: "2025-04-30 00:00:00",
        end_time: "2025-04-30 23:59:59",
        start_break: null,
        end_break: null,
        note: "30/04 - Giải phóng miền Nam"
    },
    {
        type_code: "Nghỉ lễ",
        start_time: "2025-05-01 00:00:00",
        end_time: "2025-05-01 23:59:59",
        start_break: null,
        end_break: null,
        note: "01/05 - Quốc tế Lao động"
    },
    {
        type_code: "Nghỉ lễ",
        start_time: "2025-09-02 00:00:00",
        end_time: "2025-09-02 23:59:59",
        start_break: null,
        end_break: null,
        note: "02/09 - Quốc khánh"
    },
    {
        type_code: "Nghỉ lễ",
        start_time: "2026-02-16 00:00:00",  // 29 Tết (Giao thừa)
        end_time: "2026-02-22 23:59:59",  // Mùng 6 Tết
        start_break: null,
        end_break: null,
        note: "Tết Nguyên Đán 2026 (29 AL – mùng 6 AL)"
    }
];

const loadData = async () => {
    const $table = $('#setup_table');

    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        processing: true,
        serverSide: false,
        responsive: true,
        autoWidth: false,
        searching: true,
        data: fakeData, // ✅ sử dụng JSON object

        columns: [
            { data: 'type_code', title: 'Phân loại' },
            {
                data: 'start_time',
                title: 'Bắt đầu',
                render: function (data, type, row) {
                    if (!data) return '';
                    if (row.start_break) {
                        return moment(data, "HH:mm:ss").format("HH:mm:ss");
                    } else {
                        return moment(data).format("YYYY/MM/DD");
                    }
                }
            },
            {
                data: 'end_time',
                title: 'Kết thúc',
                render: function (data, type, row) {
                    if (!data) return '';
                    if (row.start_break) {
                        return moment(data, "HH:mm:ss").format("HH:mm:ss");
                    } else {
                        return moment(data).format("YYYY/MM/DD");
                    }
                }
            },
            {
                data: 'start_break',
                title: 'Giờ nghỉ từ',
                render: data => data ? moment(data, "HH:mm:ss").format("HH:mm:ss") : ''
            },
            {
                data: 'end_break',
                title: 'Giờ nghỉ đến',
                render: data => data ? moment(data, "HH:mm:ss").format("HH:mm:ss") : ''
            },
            { data: 'note', title: 'Mô tả' },
            {
                data: null,
                orderable: false,
                className: 'text-end',
                render: (data, type, row) => `
                    <div class="dropdown">
                        <button class="btn btn-light btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown">
                            Lựa chọn
                        </button>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item text-primary" href="#">Sửa</a></li>
                            <li><a class="dropdown-item text-danger" href="#" data-kt-users-table-filter="delete_row">Xóa</a></li>
                        </ul>
                    </div>`
            }
        ],

        language: {
            lengthMenu: "Hiển thị _MENU_ bản ghi mỗi trang",
            info: "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
            paginate: {
                first: "Đầu",
                last: "Cuối",
                next: "Sau",
                previous: "Trước"
            },
            processing: "Đang xử lý..."
        }
    });
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




