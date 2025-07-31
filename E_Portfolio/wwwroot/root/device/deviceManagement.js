let listType = {};

$(document).ready(function () {
    loadDeviceType();
    loadData();
});

const loadDeviceType = async () => {

    apiHelper.get(`/Masters/DeviceType`, {},
        function (res) {
            listType = res.data;
            comboBox.renderSelectOptions('cbo_Type', res.data, 'type_id', 'type_name', 'Tất cả');
        },
        function (err) {
            console.error("Lỗi lấy danh sách");
        });
}
const searchClick = () => {
    loadData();
}
const loadData = async () => {
    const $table = $('#device_table');
    // Xóa DataTable nếu đã khởi tạo
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        processing: true,
        serverSide: true,
        responsive: true,
        autoWidth: false,
        ajax: {
            url: `${apiBase}/Device/Management`,
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                const sortColumnIndex = d.order?.[0]?.column;
                const sortDir = d.order?.[0]?.dir === 'asc';

                const columnName = d.columns?.[sortColumnIndex]?.data;

                return JSON.stringify({
                    status_id: $('#cbo_Status').val() || 0,
                    type_id: $('#cbo_Type').val() || 0,
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
                orderable: false,
                render: (data, type, row) => `
                    <div class="form-check form-check-sm form-check-custom form-check-solid">
                        <input class="form-check-input" type="checkbox" value="">
                    </div>`
            },
            { data: 'type_name', title: 'Phân loại' },
            { data: 'device_code', title: 'Mã thiết bị' },
            { data: 'device_name', title: 'Tên thiết bị' },
            { data: 'quantity', title: 'Số lượng' },
            { data: 'device_config', title: 'Cấu hình thiết bị' },
            {
                data: null,
                orderable: false,
                render: (data, type, row) => {
                    if (!row.user_code) return ``;
                    return `<div>
                            <span class="badge badge-light-info">${row.user_code}</span> 
                            <span>${row.full_name}</span>
                        </div>`;
                }
            },
            {
                data: 'status_name',
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

const sampleClick = async () => {
    const fileName = 'DeviceManagement_SampleFile.xlsx';
    const url = `${apiBase.replace(/\/api$/, '')}/Device/${fileName}`;

    // Cách đơn giản: chuyển hướng trình duyệt để tải file
    window.location.href = url;
};

const importClick = () => {
    document.getElementById('file_Import').click();
};
const onFileSelected = (event) => {
    const fileInput = event.target;
    const file = fileInput.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("file", file);

    apiHelper.postFile(
        `/Device/Import-excel`,
        formData,
        function (response) {
            // Hiển thị dữ liệu trong bảng
            renderDeviceTable(response.data, '#detail_table');

            // Hiển thị Drawer nếu có
            const drawerEl = document.querySelector('#kt_details');
            const drawerInstance = KTDrawer.getInstance(drawerEl);
            drawerInstance?.show();
        },
        function (err) {
            console.error("❌ Lỗi import:", err);
            alert("Không thể import file Excel.");
        },
        false,  // isAddToken
        false   // isBlob
    );

    // Reset input file để cho phép chọn lại cùng 1 file
    fileInput.value = null;
};

function renderDeviceTable(data, tableId) {
    const $table = $(tableId);

    // Xóa DataTable nếu đã khởi tạo
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    // Xóa nội dung tbody
    $table.find('tbody').empty();

    // Trạng thái và value tương ứng
    const statusOptions = {
        "Đang sử dụng": 1,
        "Chưa cấp phát": 2,
        "Không xác định": 3,
        "Hỏng": 4
    };


    // Duyệt từng dòng dữ liệu
    data.forEach((item, index) => {
        const selectedTypeId = listType.find(t => t.type_name === item.type_name)?.type_id || '';
        const selectedValue = statusOptions[item.status_name?.trim()] || 3;

        const row = `
            <tr>
                <td class="text-center">${index + 1}</td>
                <td>
                    <select id="cbo_Type_${index + 1}" class="form-select form-select-sm form-select-solid select-type" data-control="select2">
                        <option value="">Chọn loại</option>
                        ${listType.map(t =>
                            `<option value="${t.type_id}" ${t.type_id === selectedTypeId ? 'selected' : ''}>${t.type_name}</option>`
                        ).join('')}
                    </select>
                </td>
                <td><input type="text" id="txt_DeviceCode_${index + 1}" class="form-control form-control-sm" value="${item.device_code || ''}"></td>
                <td><input type="text" id="txt_DeviceName_${index + 1}" class="form-control form-control-sm" value="${item.device_name || ''}"></td>
                <td><textarea id="txt_DeviceConfig_${index + 1}" class="form-control form-control-sm" rows="2">${item.device_config || ''}</textarea></td>
                <td>
                    <select id="cbo_Staus_${index + 1}" class="form-select form-select-sm form-select-solid" data-control="select2" data-allow-clear="true">
                        <option value="1" ${selectedValue === 1 ? 'selected' : ''}>Đang sử dụng</option>
                        <option value="2" ${selectedValue === 2 ? 'selected' : ''}>Chưa cấp phát</option>
                        <option value="3" ${selectedValue === 3 ? 'selected' : ''}>Không xác định</option>
                        <option value="4" ${selectedValue === 4 ? 'selected' : ''}>Hỏng</option>
                    </select>
                </td>
                <td><input type="text" id="txt_UserCode_${index + 1}" class="form-control form-control-sm" value="${item.user_code || ''}"></td>
                <td><p class="">${item.full_name || ''}</p></td>
                <td><input type="text" id="txt_Note_${index + 1}" class="form-control form-control-sm" value="${item.note || ''}"></td>
                <td>
                    <button type="button" class="btn btn-icon btn-sm btn-danger btn-delete-row" onclick="deleteRowByButton(this)">
                        <i class="bi bi-x fs-2"></i>
                    </button>
                </td>
            </tr>
        `;

        $table.find('tbody').append(row);
    });

    comboBox.setSelectOptions();

    // Khởi tạo lại DataTable
    $table.DataTable({
        paging: false,
        info: false,
        searching: false,
        ordering: false,
        autoWidth: false,
        language: {
            emptyTable: "Không có dữ liệu thiết bị",
            processing: "Đang xử lý..."
        }
    });
}
function deleteRowByButton(button) {
    // Tìm <tr> gần nhất từ nút được click
    const $row = $(button).closest('tr');
    $row.remove(); // Xóa dòng

    // Lặp lại toàn bộ các dòng còn lại để cập nhật lại STT và ID
    $('#detail_table tbody tr').each((i, row) => {
        const index = i + 1;

        // Cập nhật lại STT ở cột đầu tiên
        $(row).find('td:first').text(index);

        // Cập nhật ID của các input/select để đảm bảo không trùng
        $(row).find('select[id^="cbo_Type_"]').attr('id', `cbo_Type_${index}`);
        $(row).find('input[id^="txt_DeviceCode_"]').attr('id', `txt_DeviceCode_${index}`);
        $(row).find('input[id^="txt_DeviceName_"]').attr('id', `txt_DeviceName_${index}`);
        $(row).find('textarea[id^="txt_DeviceConfig_"]').attr('id', `txt_DeviceConfig_${index}`);
        $(row).find('select[id^="cbo_Staus_"]').attr('id', `cbo_Staus_${index}`);
        $(row).find('input[id^="txt_UserCode_"]').attr('id', `txt_UserCode_${index}`);
        $(row).find('input[id^="txt_Note_"]').attr('id', `txt_Note_${index}`);

        // Cập nhật lại nút xóa để luôn gọi đúng hàng
        $(row).find('button.btn-delete-row').attr('onclick', `deleteRowByButton(this)`);
    });
}


const exportClick = async () => {
    apiHelper.post(`/Device/Export-excel`,
        {
            status_id: $('#cbo_Status').val() || 0,
            type_id: $('#cbo_Type').val() || 0,
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




