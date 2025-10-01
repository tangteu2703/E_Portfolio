var fromDate, toDate;

$(document).ready(function () {

    setDefaultDateRange();
    comboBox.setSelectOptions();

    loadData();
    loadUser();
});

const setDefaultDateRange = () => {
    var month = parseInt($('#cbo_Month').val()); // 1–12
    var year = parseInt($('#cbo_Year').val());

    // Tạo ngày đầu tiên của tháng
    fromDate = moment(`${year}-${month}-01`, "YYYY-MM-DD").startOf('month').format('YYYY-MM-DD');

    // Tạo ngày cuối cùng của tháng
    toDate = moment(`${year}-${month}-01`, "YYYY-MM-DD").endOf('month').format('YYYY-MM-DD');

    console.log("From:", fromDate, "To:", toDate);
};

const loadUser = async () => {

    apiHelper.get(`/Masters/Users`, {},
        function (res) {
            comboBox.renderSelectOptions('cbo_User', res.data, 'user_code', 'full_name', 'Tất cả');
        },
        function (err) {
            console.error("Lỗi lấy danh sách");
        });
}

const searchClick = () => {
    loadData();
}

const loadData = async () => {
    const $table = $('#worksheet_table');

    // Hủy DataTable nếu đã khởi tạo trước đó
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    $table.DataTable({
        processing: true,
        serverSide: true,
        responsive: true,
        autoWidth: false,
        searching: false, // tắt search mặc định (vì mình dùng search riêng)
        ajax: {
            url: `${apiBase}/WorkSheet/TimeSheet`,
            type: 'POST',
            contentType: 'application/json',
            data: function (d) {
                const sortColumnIndex = d.order?.[0]?.column ?? 0;
                const sortDir = d.order?.[0]?.dir ?? 'asc';
                const columnName = d.columns?.[sortColumnIndex]?.data ?? "";

                // Nếu chọn "Hiển thị tất cả" thì pageSize = 0
                let pageSize = d.length;
                if (pageSize === -1) pageSize = 0;

                return JSON.stringify({
                    from_date: fromDate || null,
                    to_date: toDate || null,
                    textSearch: $('#cbo_User').val() || "",
                    pageNumber: (d.start / d.length) + 1,
                    pageSize: 0,
                    orderBy: columnName,
                    isAscending: sortDir === 'asc'
                });
            },
            dataSrc: function (json) {
                // Chuẩn hóa dữ liệu trả về cho DataTable
                return json?.data || [];
            },
            error: function (xhr, error, thrown) {
                console.error("Lỗi tải dữ liệu:", error, xhr.responseText);
            }
        },
        columns: [
            { data: 'user_code', title: 'Mã NV' },
            { data: 'full_name', title: 'Họ tên' },
            { data: 'total', title: 'Tổng' },
            { data: 'day_01', title: '01' },
            { data: 'day_02', title: '02' },
            { data: 'day_03', title: '03' },
            { data: 'day_04', title: '04' },
            { data: 'day_05', title: '05' },
            { data: 'day_06', title: '06' },
            { data: 'day_07', title: '07' },
            { data: 'day_08', title: '08' },
            { data: 'day_09', title: '09' },
            { data: 'day_10', title: '10' },
            { data: 'day_11', title: '11' },
            { data: 'day_12', title: '12' },
            { data: 'day_13', title: '13' },
            { data: 'day_14', title: '14' },
            { data: 'day_15', title: '15' },
            { data: 'day_16', title: '16' },
            { data: 'day_17', title: '17' },
            { data: 'day_18', title: '18' },
            { data: 'day_19', title: '19' },
            { data: 'day_20', title: '20' },
            { data: 'day_21', title: '21' },
            { data: 'day_22', title: '22' },
            { data: 'day_23', title: '23' },
            { data: 'day_24', title: '24' },
            { data: 'day_25', title: '25' },
            { data: 'day_26', title: '26' },
            { data: 'day_27', title: '27' },
            { data: 'day_28', title: '28' },
            { data: 'day_29', title: '29' },
            { data: 'day_30', title: '30' },
            { data: 'day_31', title: '31' },
        ],
        language: {
            emptyTable: "Không có dữ liệu trong bảng",
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
        `/WorkSheet/Import-TimeSheet`,
        formData,
        function (response) {
            // Hiển thị dữ liệu trong bảng
            loadData();
        },
        function (err) {
            console.error("❌ Lỗi import:", err);
            alert("Lỗi khi import file Excel.");
        },
        false,  // isAddToken
        false   // isBlob
    );

    // Reset input file để cho phép chọn lại cùng 1 file
    fileInput.value = null;
};

const exportClick = async () => {

    apiHelper.post(`/WorkSheet/Export-TimeSheet`,
        {
            from_date: fromDate || null,
            to_date: toDate || null,
            textSearch: $('#cbo_User').val() || "",
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


