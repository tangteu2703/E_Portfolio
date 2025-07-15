$(document).ready(function () {

    loadData();

});
const searchClick = () => {
    loadData();
}
const loadData = async () => {
    const $table = $('#worksheet_table');

    // Xóa DataTable nếu đã khởi tạo
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        processing: true,
        serverSide: true, // Kích hoạt phân trang phía server
        responsive: true,
        autoWidth: false,
        ajax: {
            url: `${apiBase}/WorkSheet/Setup`,
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                const sortColumnIndex = d.order?.[0]?.column;
                const sortDir = d.order?.[0]?.dir === 'asc';

                const columnName = d.columns?.[sortColumnIndex]?.data;

                return JSON.stringify({
                    dept_code: $('#cbo_Dept').val() || "",
                    shift: $('#cbo_Shift').val() || "",
                    from_date: $('#txt_FromDate').val() || null,
                    to_date: $('#txt_ToDate').val() || null,
                    textSearch: $('#txt_Search').val() || "",
                    pageNumber: (d.start / d.length) + 1,
                    pageSize: d.length,
                    orderBy: columnName,
                    isAscending: !sortDir
                });
            },
            dataSrc: function (json) {
                // DataTables yêu cầu phải trả về tổng số bản ghi để phân trang
                //console.log(json.data);
                return json.data;
            },
            error: function (xhr, error, thrown) {
                console.error("Lỗi tải dữ liệu:", error, xhr.responseText);
            }
        },
        columns: [
            {
                data: null,
                orderable: false, // Không cho sắp xếp cột checkbox
                render: (data, type, row) => `
                    <div class="form-check form-check-sm form-check-custom form-check-solid">
                        <input class="form-check-input" type="checkbox" value="${row.emp_Code}">
                    </div>`
            },
            {
                data: 'emp_Code',
                render: (data, type, row) => `
                    <div class="d-flex align-items-center">
                        <div class="symbol symbol-circle symbol-30px overflow-hidden me-3">
                            <a href="#">
                                <div class="symbol-label">
                                    <img src="https://picsum.photos/200/300" alt="avt" class="w-100">
                                </div>
                            </a>
                        </div>
                        <div class="d-flex flex-column">
                            <a href="#" class="text-gray-800 text-hover-primary mb-1">${row.emp_Name}</a>
                            <span>${row.emp_Code}</span>
                        </div>
                    </div>`
            },
            { data: 'dept_Name' },
            {
                data: 'work_Day',
                render: data => data ? moment(data).format("YYYY/MM/DD") : ''
            },
            {
                data: 'dateTime_In',
                render: data => data ? moment(data).format("YYYY/MM/DD HH:mm:ss") : ''
            },
            {
                data: 'dateTime_Out',
                render: data => data ? moment(data).format("YYYY/MM/DD HH:mm:ss") : ''
            },
            { data: 'shift' },
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
                            <li><a class="dropdown-item" href="#" onclick="viewHistory('${row.emp_Code}','${row.dateTime_In ?? row.dateTime_Out}')">Lịch sử In/Out</a></li>
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

const viewHistory = async (emp_code, dateTime) => {
    const $table = $('#zktbio_table');

    // Xóa DataTable nếu đã khởi tạo
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        processing: true,
        responsive: false,
        autoWidth: false,
        paging: false,        // Ẩn phân trang
        info: false,          // Ẩn dòng "Showing x of y entries"
        lengthChange: false,  // Ẩn dropdown số dòng mỗi trang
        searching: true,       // Hiện ô tìm kiếm
        ordering: false,         // Ẩn tính năng sắp xếp cột
        ajax: {
            url: `${apiBase}/WorkSheet/History-Bio`,
            type: 'POST',
            contentType: 'application/json',
            data: function () {
                return JSON.stringify({
                    from_date: dateTime,
                    textSearch: emp_code,
                    pageNumber: 1,
                    pageSize: 1000,
                    isAscending: false
                });
            },
            dataSrc: function (json) {
                return json.data;
            },
            error: function (xhr, error, thrown) {
                console.error("Lỗi tải dữ liệu:", error, xhr.responseText);
            }
        },
        columns: [
            { data: 'emp_code' },
            { data: 'terminal_alias' },
            {
                data: 'punch_time',
                render: data => data ? moment(data).format("YYYY/MM/DD HH:mm:ss") : ''
            },
            { data: 'area_alias' },
            { data: 'terminal_sn' },
            {
                data: 'upload_time',
                render: data => data ? moment(data).format("YYYY/MM/DD HH:mm:ss") : ''
            },
           
        ],
        language: {
            lengthMenu: "Hiển thị _MENU_ bản ghi",
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

    KTDrawer.getInstance($("#kt_activities")[0])?.show();

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



