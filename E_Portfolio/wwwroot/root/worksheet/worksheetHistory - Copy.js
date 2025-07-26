$(document).ready(function () {

    loadData();
    loadDept();

    comboBox.setSelectOptions();
});
const loadDept = async () => {

    apiHelper.get(`/Masters/Departments`, {},
        function (res) {
            comboBox.renderSelectOptions('cbo_Dept', res.data, 'dept_code', 'dept_name', 'Tất cả');
        },
        function (err) {
            console.error("Lỗi lấy danh sách");
        });
}
const searchClick = () => {
    loadData();
}
const importClick = () => {
    document.getElementById('file_Import').click();
}

const onImageSelected = (event) => {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function () {
        const base64 = reader.result.split(',')[1];

        apiHelper.post(`/Image/detect-plate`,
            { base64: base64 },
            function (res) {
                console.log("Kết quả nhận diện:", res);
                const html = `
                    <img src="data:image/jpeg;base64,${res.annotated_image}" style="width:100%; height: 60vh; border:1px solid #ccc;" />
                `;
                document.getElementById("resultContent").innerHTML = html;

                // Mở modal
                const modal = new bootstrap.Modal(document.getElementById("resultModal"));
                modal.show();
            },
            function (err) {
                console.error("Lỗi detect plate:", err);
                alert("Không thể nhận diện ảnh.");
            });
    };

    reader.readAsDataURL(file);
};

const resultModal = document.getElementById('resultModal');
resultModal.addEventListener('hidden.bs.modal', function () {
    document.getElementById('resultContent').innerHTML = '';
});

const loadData = async () => {
    const $table = $('#worksheet_table');

    // Xóa DataTable nếu đã khởi tạo
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        processing: true,
        responsive: true,
        autoWidth: false,
        ajax: {
            url: `${apiBase}/WorkSheet/History`,
            type: 'POST',
            contentType: 'application/json',
            data: function () {
                return JSON.stringify({
                    dept_code: $('#cbo_Dept').val() || "",
                    shift: $('#cbo_Shift').val() || "",
                    from_date: $('#txt_FromDate').val() || null,
                    to_date: $('#txt_ToDate').val() || null,
                    textSearch: $('#txt_Search').val() || "",
                    pageNumber: 1,
                    pageSize: 20000,
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
            {
                data: null,
                orderable: false,
                render: (data, type, row) => `
                    <div class="form-check form-check-sm form-check-custom form-check-solid">
                        <input class="form-check-input" type="checkbox" value="${row.Emp_Code}">
                    </div>`
            },
            {
                data: null,
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
                            <a href="#" class="text-gray-800 text-hover-primary mb-1">${row.Emp_Name}</a>
                            <span>${row.Emp_Code}</span>
                        </div>
                    </div>`
            },
            { data: 'Dept_Name' },
            {
                data: 'Work_Day',
                render: data => data ? moment(data).format("YYYY/MM/DD") : ''
            },
            {
                data: 'DateTime_In',
                render: data => data ? moment(data).format("YYYY/MM/DD HH:mm:ss") : ''
            },
            {
                data: 'DateTime_Out',
                render: data => data ? moment(data).format("YYYY/MM/DD HH:mm:ss") : ''
            },
            {
                data: null,
                render: (data, type, row) => calculateOvertime(row.DateTime_In, row.DateTime_Out, row.Shift)
            },
            { data: 'Shift' },
            {
                data: null,
                orderable: false,
                className: 'text-end',
                render: () => `
                    <div class="dropdown">
                        <button class="btn btn-light btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown">
                            Lựa chọn
                        </button>
                        <ul class="dropdown-menu">
                            <li><a class="dropdown-item" href="#">Sửa</a></li>
                            <li><a class="dropdown-item text-danger" href="#" data-kt-users-table-filter="delete_row">Xóa</a></li>
                        </ul>
                    </div>`
            }
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
};


const exportClick = async () => {
    apiHelper.post(`/WorkSheet/Export-excel`,
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

// Tính thời gian tăng ca theo ca làm việc (ngày hoặc đêm). v123
function calculateOvertime(inTimeStr, outTimeStr, shift = 'Ca ngày') {
    const inTime = moment(inTimeStr);
    const outTime = moment(outTimeStr);

    let startShift, endShift;
    if (shift == null)
        shift = 'Ca ngày'

    if (shift.toLowerCase().includes('ngày')) {
        startShift = inTime.clone().set({ hour: 7, minute: 0, second: 0 });
        endShift = inTime.clone().set({ hour: 17, minute: 0, second: 0 });
    } else {
        startShift = inTime.clone().set({ hour: 20, minute: 0, second: 0 });
        if (inTime.hour() < 5) {
            startShift.subtract(1, 'day');
        }
        endShift = startShift.clone().add(9, 'hours');
    }

    let totalMinutes = 0;

    // Tăng ca trước ca
    if (inTime.isBefore(startShift)) {
        const earlyMinutes = startShift.diff(inTime, 'minutes');
        if (earlyMinutes >= 60) {
            totalMinutes += Math.floor(earlyMinutes / 30) * 30;
        }
    }

    // Tăng ca sau ca
    if (outTime.isAfter(endShift)) {
        const lateMinutes = outTime.diff(endShift, 'minutes');
        if (lateMinutes >= 60) {
            totalMinutes += Math.floor(lateMinutes / 30) * 30;
        }
    }

    if (totalMinutes < 60) return "00.00";

    const hours = Math.floor(totalMinutes / 60);
    const minutes = totalMinutes % 60;

    return `${hours.toString().padStart(2, '0')}.${minutes === 0 ? '00' : '30'}`;
}



