
var userArr = [], userSelected = "";

$(document).ready(function () {
    init();
});

async function init() {
    setDefaultDateRange();
    loadDept();
    comboBox.setSelectOptions();
    await loadUserList();
}
const setDefaultDateRange = () => {
    const firstDay = moment().startOf('month').format('YYYY-MM-DD');
    const lastDay = moment().endOf('month').format('YYYY-MM-DD');

    $('#txt_FromDate').val(firstDay);
    $('#txt_ToDate').val(lastDay);
};

$('#txt_Search').on('keydown', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        loadUserList();
    }
});
const searchClick = () => {
    loadData();
}

const loadDept = async () => {

    apiHelper.get(`/Masters/Departments`, {},
        function (res) {
            comboBox.renderSelectOptions('cbo_Dept', res.data, 'dept_code', 'dept_name', 'Tất cả');
        },
        function (err) {
            console.error("Lỗi lấy danh sách");
        });
}

const loadUserList = async () => {
    apiHelper.get(`/User/SelectAll`,
        {
            Dept_code: $('#cbo_Dept').val() || "",
            TextSearch: $('#txt_Search').val() || "",
        },
        function (res) {
            const userList = document.getElementById('userList');
            userList.innerHTML = '';

            res.data.forEach((user, index) => {
                const userItem = document.createElement('div');
                userItem.className = 'user-item d-flex align-items-center justify-content-between py-2 px-2 border-bottom';

                userItem.innerHTML = `
                    <div class="form-check form-check-sm me-3">
                        <input class="form-check-input user-checkbox" type="checkbox" value="${user.Usercode}">
                    </div>
                    <div class="d-flex align-items-center flex-grow-1">
                        <div class="symbol symbol-30px symbol-circle">
                            <img id="avatar_${user.Usercode}" src="https://picsum.photos/200/300" alt="${user.Username}" />
                        </div>
                        <div class="ms-3">
                            <div id="username_${user.Usercode}" class="fw-bold text-gray-900 small">${user.Username}</div>
                            <div id="usercode_${user.Usercode}" class="text-muted small">${user.Usercode}</div>
                        </div>
                    </div>
                    <div id="dept_${user.Usercode}" class="text-muted small ms-auto text-end" style="min-width: 100px;">
                        ${user.DepartmentName}
                    </div>
                `;

                const checkbox = userItem.querySelector('.user-checkbox');

                // ✅ Mặc định tích chọn user đầu tiên
                if (index === 0) {
                    checkbox.checked = true;
                    userItem.classList.add('bg-light-primary');
                    userSelected = user.Usercode;
                    userArr = [user.Usercode];
                    loadData();
                }

                // Checkbox change: chỉ update danh sách userArr
                checkbox.addEventListener('change', () => {
                    const value = checkbox.value;

                    if (checkbox.checked) {
                        if (!userArr.includes(value)) {
                            userArr.push(value);
                        }
                    } else {
                        userArr = userArr.filter(id => id !== value);
                    }

                    console.log("Danh sách đã chọn:", userArr);
                    console.log("User đã chọn:", userSelected);
                });

                // Click dòng: toggle nền + check
                userItem.addEventListener('click', function (e) {
                    if (e.target.tagName.toLowerCase() === 'input') return;

                    const checkbox = userItem.querySelector('.user-checkbox');
                    const isSelected = userItem.classList.contains('bg-light-primary');

                    if (isSelected) {
                        userItem.classList.remove('bg-light-primary');
                        checkbox.checked = false;
                        userArr = userArr.filter(id => id !== checkbox.value);
                        userSelected = '';
                    } else {
                        document.querySelectorAll('.user-item').forEach(item => {
                            item.classList.remove('bg-light-primary');
                            const cb = item.querySelector('.user-checkbox');
                            if (cb) cb.checked = false;
                        });

                        userItem.classList.add('bg-light-primary');
                        checkbox.checked = true;

                        userArr = [checkbox.value];
                        userSelected = checkbox.value;

                        loadData();
                    }

                    //console.log("Danh sách đã chọn:", userArr);
                    console.log("User đã chọn:", userSelected);

                });

                userList.appendChild(userItem);
            });

            // Xử lý checkbox "Chọn tất cả"
            const selectAll = document.getElementById('selectAllUser');
            if (selectAll) {
                selectAll.addEventListener('change', function () {
                    const isChecked = this.checked;
                    document.querySelectorAll('.user-checkbox').forEach(cb => {
                        cb.checked = isChecked;
                        cb.dispatchEvent(new Event('change'));
                    });
                });
            }

            // Cập nhật trạng thái nút "Chọn tất cả"
            document.querySelectorAll('.user-checkbox').forEach(cb => {
                cb.addEventListener('change', function () {
                    const all = document.querySelectorAll('.user-checkbox');
                    const allChecked = Array.from(all).every(c => c.checked);
                    const selectAll = document.getElementById('selectAllUser');
                    if (selectAll) {
                        selectAll.checked = allChecked;
                    }
                });
            });
        },
        function (err) {
            console.error("Lỗi lấy danh sách");
        });
};

const loadData = async () => {
    const $table = $('#worksheet_table');

    // Nếu bảng đã khởi tạo trước đó, xóa trước khi render lại
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        processing: true,
        responsive: true,
        autoWidth: false,
        paging: false,
        ordering: false,
        info: false,
        searching: false,

        ajax: {
            url: `${apiBase}/WorkSheet/Detail`,
            type: 'POST',
            contentType: 'application/json',
            data: function () {
                return JSON.stringify({
                    from_date: $('#txt_FromDate').val() || null,
                    to_date: $('#txt_ToDate').val() || null,
                    textSearch: userSelected,
                    pageNumber: 1,
                    pageSize: 1000,
                    isAscending: false
                });
            },
            dataSrc: function (json) {
                const data = json.data || [];
                const from = new Date($('#txt_FromDate').val());
                const to = new Date($('#txt_ToDate').val());

                const dataMap = new Map();
                data.forEach(item => {
                    const dateStr = item.work_Day?.substring(0, 10);
                    dataMap.set(dateStr, item);
                });

                const fullData = [];

                for (let d = new Date(from); d <= to; d.setDate(d.getDate() + 1)) {
                    const dateStr = d.toISOString().substring(0, 10);

                    if (dataMap.has(dateStr)) {
                        fullData.push(dataMap.get(dateStr));
                    } else {
                        const day = d.getDay(); // 0: CN, 6: T7
                        const weekNumber = getWeekNumber(d);

                        // Chủ nhật luôn là Weekend, Thứ 7 là Weekend nếu tuần lẻ
                        const isWeekend = (day === 0) || (day === 6 && weekNumber % 2 === 1);

                        fullData.push({
                            id: 0,
                            work_Day: dateStr,
                            shift_Code: "000",
                            day_Code: isWeekend ? "Weekend" : "Weekday",
                            work_Hour: "",
                            lack_Hour: "",
                            time_In: "",
                            time_Out: "",
                            oT_101: "",
                            oT_102: "",
                            oT_103: "",
                            oT_201: "",
                            oT_202: "",
                            oT_301: "",
                            oT_302: "",
                            note: "",
                        });
                    }
                }

                return fullData;

                // Tính số tuần trong năm
                function getWeekNumber(date) {
                    const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
                    const dayNum = d.getUTCDay() || 7;
                    d.setUTCDate(d.getUTCDate() + 4 - dayNum);
                    const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
                    return Math.ceil((((d - yearStart) / 86400000) + 1) / 7);
                }
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
                        <input class="form-check-input" type="checkbox" value="${row.work_Day}">
                    </div>`
            },
            {
                data: 'work_Day',
                render: data => data ? moment(data).format("YYYY/MM/DD") : ''
            },
            {
                data: 'work_Day',
                render: function (data, type, row) {
                    if (!data || row.isSeparator) return data || '';

                    const day = moment(data).day(); // 0: Sunday, 6: Saturday
                    const thuVN = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
                    const label = thuVN[day];

                    if (row.day_Code != "Weekday") {
                        return `<span class="text-danger">${label}</span>`;
                    }

                    return label;
                }
            },
            {
                data: 'shift_Code',
                render: function (data, type, row) {
                    switch (data) {
                        case '001': return 'Ngày';
                        case '002': return 'Đêm';
                        case '000':
                        case null:
                        case undefined:
                        case '': return '';
                        default: return data; // fallback nếu có mã khác
                    }
                }
            },
            {
                data: 'time_In',
                render: data => data ? moment(data).format("HH:mm:ss") : ''
            },
            {
                data: 'time_Out',
                render: data => data ? moment(data).format("HH:mm:ss") : ''
            },
            { data: 'lack_Hour' },
            { data: 'work_Hour' },
            { data: 'oT_101' },
            { data: 'oT_102' },
            { data: 'oT_103' },
            { data: 'oT_201' },
            { data: 'oT_202' },
            { data: 'oT_301' },
            { data: 'oT_302' },
            { data: 'note' },
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
                            <li><a class="dropdown-item" href="#" onclick="viewDetail('${row.emp_Code}','${row.time_In ?? ``}','${row.time_Out ?? ``}')">Cập nhật</a></li>
                            <li><a class="dropdown-item text-danger" href="#" data-kt-users-table-filter="delete_row">Xóa bỏ</a></li>
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

const viewDetail = async (emp_code, time_In,time_Out) => {
    // 1. Lấy avatar src
    let avatarSrc = $(`#avatar_${emp_code}`).attr('src') || '';
    $('#avatar_kt').attr('src', avatarSrc);

    // 2. Lấy tên người dùng
    let username = $(`#username_${emp_code}`).text().trim() || '';
    $('#username_kt').text(username);

    // 3. Lấy mã nhân viên
    let usercode = $(`#usercode_${emp_code}`).text().trim() || '';
    $('#usercode_kt').text(usercode);

    // 4. Lấy 
    let dept = $(`#dept_${emp_code}`).text().trim() || '';
    $('#dept_kt').text(dept);

    // 5. Xét giá trị giờ vào/ra dùng moment.js theo múi giờ Việt Nam
    $('#dat_TimeIn').val('');
    $('#dat_TimeOut').val('');
    if (time_In) {
        const timeInFormatted = moment(time_In).utcOffset('+07:00').format('YYYY-MM-DDTHH:mm');
        $('#dat_TimeIn').val(timeInFormatted);
    }

    if (time_Out) {
        const timeOutFormatted = moment(time_Out).utcOffset('+07:00').format('YYYY-MM-DDTHH:mm');
        $('#dat_TimeOut').val(timeOutFormatted);
    }

    // 4. Hiển thị Drawer
    KTDrawer.getInstance(document.querySelector('#kt_details'))?.show();

    // 5. Load lịch sử In/Out
    var dateTime = time_In && time_In.length > 0 ? time_In : time_Out;
    viewHistory(emp_code, dateTime);
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
};

const saveDetail = async () => {
    const empCode = $('#usercode_kt').text().trim();

    const timeIn = $('#dat_TimeIn').val();
    const timeOut = $('#dat_TimeOut').val();

    if (!timeIn || !timeOut) {
        Swal.fire({
            text: "Vui lòng nhập đầy đủ giờ vào và giờ ra!",
            icon: "warning",
            buttonsStyling: false,
            confirmButtonText: "Ok, tôi hiểu!",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        });
        return;
    }

    const data = {
        emp_code: empCode,
        work_hour: parseFloat($('#work_hour').val()) || 0,
        lack_hour: parseFloat($('#lack_hour').val()) || 0,
        time_in: timeIn,
        time_out: timeOut,
        ot_101: parseFloat($('#ot_150').val()) || 0,
        ot_102: parseFloat($('#ot_210').val()) || 0,
        ot_103: parseFloat($('#ot_200').val()) || 0,
        ot_201: parseFloat($('#ot_270').val()) || 0,
        ot_202: parseFloat($('#ot_270').val()) || 0,
        ot_301: parseFloat($('#ot_300').val()) || 0,
        ot_302: parseFloat($('#ot_390').val()) || 0,
    };

    if (
        data.work_hour <= 0 &&
        data.ot_150 <= 0 &&
        data.ot_210 <= 0 &&
        data.ot_200 <= 0 &&
        data.ot_270 <= 0 &&
        data.ot_300 <= 0 &&
        data.ot_390 <= 0
    ) {
        Swal.fire({
            text: "Vui lòng tính toán nhập ít nhất một giá trị giờ công hoặc OT > 0!",
            icon: "warning",
            buttonsStyling: !1,
            confirmButtonText: "Ok, tôi hiểu!",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        });
        return;
    }

    try {
        await apiHelper.put(`/WorkSheet/Update-Detail`, data,
            function (res) {
                Swal.fire({
                    text: "Cập nhật thành công!",
                    icon: "success",
                    buttonsStyling: !1,
                    confirmButtonText: "Ok, tôi hiểu!",
                    customClass: {
                        confirmButton: "btn fw-bold btn-primary"
                    }
                });
            },
            function (err) {
                console.error("Lỗi khi cập nhật chi tiết:", err);
                toastr.error("Cập nhật thất bại!");
            });
    } catch (e) {
        console.error("Lỗi không xác định:", e);
    }
};

const exportClick = async () => {

    apiHelper.post(`/WorkSheet/Export-Detail`,
        {
            dept_code: $('#cbo_Dept').val() || "",
            from_date: $('#txt_FromDate').val() || null,
            to_date: $('#txt_ToDate').val() || null,
            textSearch: userArr.join(','),
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



