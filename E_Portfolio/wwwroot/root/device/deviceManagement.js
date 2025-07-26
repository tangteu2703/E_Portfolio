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
    formData.append("file", file); // "file" là tên tham số trong controller

    apiHelper.postFile(
        `/Device/Import-excel`,
        formData,
        function (res) {
            console.log("Import thành công:", res);

            // Gán dữ liệu nếu cần
            // $('#importResult').text(JSON.stringify(res, null, 2));

            // Hiển thị modal kết quả
            const modal = new bootstrap.Modal(document.getElementById("resultModal"));
            modal.show();
        },
        function (err) {
            console.error("Lỗi import:", err);
            alert("Không thể import file Excel.");
        }
    );
};


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




