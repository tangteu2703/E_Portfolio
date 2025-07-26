$(document).ready(function () {
    loadDeviceType();
    loadData();

});

const loadDeviceType = async () => {

    apiHelper.get(`/Masters/DeviceType`, {},
        function (res) {
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
    const file = event.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("file", file);

    apiHelper.postFile(
        `/Device/Import-excel`,
        formData,
        function (response) {
            console.log("✅ Import thành công:", response);
            renderDeviceTable(response.data, '#detail_table')
            // 4. Hiển thị Drawer
            KTDrawer.getInstance(document.querySelector('#kt_details'))?.show();
        },
        function (err) {
            console.error("❌ Lỗi import:", err);
            alert("Không thể import file Excel.");
        },
        false,  // isAddToken
        false, // isBlob
    );
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
        const selectedValue = statusOptions[item.status_name?.trim()] || 0;

        const row = `
            <tr>
                <td class="text-center">${index + 1}</td>
                <td><input type="text" class="form-control form-control-sm" value="${item.type_name || ''}"></td>
                <td><input type="text" class="form-control form-control-sm" value="${item.device_code || ''}"></td>
                <td><input type="text" class="form-control form-control-sm" value="${item.device_name || ''}"></td>
                <td><textarea class="form-control form-control-sm" rows="2">${item.device_config || ''}</textarea></td>
                <td>
                    <select class="form-select form-select-sm form-select-solid" data-control="select2" data-allow-clear="true">
                        <option value="1" ${selectedValue === 1 ? 'selected' : ''}>Đang sử dụng</option>
                        <option value="2" ${selectedValue === 2 ? 'selected' : ''}>Chưa cấp phát</option>
                        <option value="3" ${selectedValue === 3 ? 'selected' : ''}>Không xác định</option>
                        <option value="4" ${selectedValue === 4 ? 'selected' : ''}>Hỏng</option>
                    </select>
                </td>
                <td><input type="text" class="form-control form-control-sm" value="${item.user_code || ''}"></td>
                <td><input type="text" class="form-control form-control-sm" value="${item.full_name || ''}"></td>
                <td><input type="text" class="form-control form-control-sm" value="${item.note || ''}"></td>
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




