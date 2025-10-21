// ============================================================
// IT Request System - Layout 2 cột (List + Detail)
// ============================================================

$(() => {
    init();
});


// ==== Init ====
const init = () => {
    renderRequestList();
    updateFilterCounts();
    bindEvents();
};

// ==== Global State ====
let requestData = [];
let currentFilter = 'all';
let currentRequestId = null;

// ==== Fake Data ====
const generateFakeData = () => [
    {
        request_id: "REQ-20251016-001", request_date: "2025/10/16", deadline: "2025/10/17",
        requester_code: "NV001", requester_name: "Nguyễn Văn A",
        requester_position: "Nhân viên IT",
        dept: "IT", receive_dept: "IT",
        title: "Yêu cầu cấp laptop mới", content: "Laptop cũ bị hỏng, cần thay thế để làm việc",
        status: "Request", step: 1,
        equipment: [
            { no: 1, nv_id: "NV001", name: "Nguyễn Văn A", device: "Laptop Dell Latitude 5420", purpose: "Làm việc văn phòng" }
        ],
        incharge_email: "nguyen.vana@company.vn",
        attachments: ["laptop_specs.pdf"]
    },
    {
        request_id: "REQ-20251016-002", request_date: "2025/10/15", deadline: "2025/10/16",
        requester_code: "NV002", requester_name: "Trần Thị B",
        requester_position: "Kế toán",
        dept: "Kế toán", receive_dept: "IT",
        title: "Cần máy tính để bàn cho phòng kế toán", content: "Phòng kế toán thiếu 1 máy tính để bàn",
        status: "Approved", step: 2,
        equipment: [
            { no: 1, nv_id: "NV002", name: "Trần Thị B", device: "PC Desktop HP", purpose: "Công việc kế toán" }
        ],
        incharge_email: "tran.thib@company.vn",
        attachments: ["pc_specs.docx"]
    },
    {
        request_id: "REQ-20251016-003", request_date: "2025/10/14", deadline: "2025/10/15",
        requester_code: "NV003", requester_name: "Lê Văn C",
        requester_position: "Nhân viên kinh doanh",
        dept: "Kinh doanh", receive_dept: "IT",
        title: "Yêu cầu cấp máy tính xách tay cho công tác kinh doanh", content: "Cần laptop để đi gặp khách hàng",
        status: "Ongoing", step: 3,
        equipment: [
            { no: 1, nv_id: "NV003", name: "Lê Văn C", device: "Laptop Lenovo ThinkPad", purpose: "Công tác kinh doanh" }
        ],
        incharge_email: "le.vanc@company.vn",
        attachments: ["laptop_request.xlsx"]
    },
    {
        request_id: "REQ-20251016-004", request_date: "2025/10/13", deadline: "2025/10/14",
        requester_code: "NV004", requester_name: "Phạm Thị D",
        requester_position: "Trưởng phòng nhân sự",
        dept: "Nhân sự", receive_dept: "IT",
        title: "Cần thêm màn hình máy tính cho phòng nhân sự", content: "Thiếu màn hình để làm việc hiệu quả hơn",
        status: "Confirm", step: 4,
        equipment: [
            { no: 1, nv_id: "NV004", name: "Phạm Thị D", device: "Màn hình Dell 24 inch", purpose: "Làm việc nhân sự" }
        ],
        incharge_email: "pham.thid@company.vn",
        attachments: ["monitor_specs.pdf"]
    },
    {
        request_id: "REQ-20251016-005", request_date: "2025/10/12", deadline: "2025/10/13",
        requester_code: "NV005", requester_name: "Hoàng Văn E",
        requester_position: "Kỹ thuật viên",
        dept: "Kỹ thuật", receive_dept: "IT",
        title: "Yêu cầu cấp máy tính chuyên dụng cho thiết kế", content: "Cần máy tính có cấu hình cao để thiết kế CAD",
        status: "Done", step: 5,
        equipment: [
            { no: 1, nv_id: "NV005", name: "Hoàng Văn E", device: "Workstation HP Z4", purpose: "Thiết kế kỹ thuật" }
        ],
        incharge_email: "hoang.vane@company.vn",
        attachments: ["workstation_specs.pdf"]
    },
    {
        request_id: "REQ-20251016-006", request_date: "2025/10/11", deadline: "2025/10/12",
        requester_code: "NV006", requester_name: "Đỗ Thị F",
        requester_position: "Nhân viên marketing",
        dept: "Marketing", receive_dept: "IT",
        title: "Cần laptop để làm việc từ xa", content: "Làm việc remote cần thiết bị di động",
        status: "Rejected", step: 2,
        equipment: [
            { no: 1, nv_id: "NV006", name: "Đỗ Thị F", device: "MacBook Pro 13 inch", purpose: "Làm việc marketing" }
        ],
        incharge_email: "do.thif@company.vn",
        attachments: ["laptop_request.pdf"],
        reject_reason: "Công ty không hỗ trợ thiết bị Apple"
    }
];

// ==== Render Request List ====
const renderRequestList = () => {
    const filtered = requestData.filter(r => {
        if (currentFilter === 'all') return true;
        return r.status === currentFilter;
    });

    const html = filtered.map(r => {
        const statusClass = getStatusClass(r.status);
        const isActive = currentRequestId === r.request_id;
        return `
        <div class="px-4 py-3 border-bottom border-start border-3 ${isActive ? 'border-primary bg-light-primary' : 'border-transparent'} cursor-pointer hover-light-primary transition" data-id="${r.request_id}">
            <div class="d-flex justify-content-between align-items-start mb-2">
                <div class="flex-grow-1">
                    <div class="fw-bold text-gray-800 fs-7">${r.request_date} | ${r.request_id}</div>
                    <div class="text-primary fw-semibold fs-8">${r.dept}</div>
                </div>
                <span class="badge badge-${statusClass} badge-sm">${r.status}</span>
            </div>
            <div class="text-gray-700 fw-semibold mb-2 fs-7">${r.title}</div>
            <div class="d-flex align-items-center text-muted fs-8">
                <div class="symbol symbol-circle symbol-25px me-2">
                    <div class="symbol-label bg-light-primary text-primary fw-bold fs-8">${r.requester_name.charAt(0)}</div>
                </div>
                <span class="text-truncate">${r.requester_name}</span>
            </div>
        </div>`;
    }).join('');

    $('#request_list_container').html(html || '<div class="text-center text-muted py-5">No requests found</div>');
};

const getStatusClass = (status) => {
    const map = {
        'Accept': 'warning', 'Approved': 'success', 'Ongoing': 'info',
        'Confirm': 'success', 'Closed': 'dark', 'Rejected': 'danger',
        'E_COMPLETION': 'primary'
    };
    return map[status] || 'secondary';
};

// ==== Render Detail ====
const renderRequestDetail = (requestId) => {
    const req = requestData.find(r => r.request_id === requestId);
    if (!req) return;

    currentRequestId = requestId;
    renderRequestList(); // refresh list to show active

    $('#detail_request_code').text(req.request_id + ' - ' + req.title);
    $('#detail_request_title').text(req.dept);

    // Show/hide action buttons based on status
    $('#btn_approve, #btn_view, #btn_edit, #btn_delete').addClass('d-none');
    if (req.status === 'Request') $('#btn_approve').removeClass('d-none');
    $('#btn_view, #btn_edit, #btn_delete').removeClass('d-none');

    const html = `
        <!-- Section A: Request Information -->
        <div class="bg-light-primary border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
            A. Thông tin yêu cầu thiết bị
        </div>
        <div class="mb-5">
            <div class="d-flex py-2 border-bottom"><div class="text-muted" style="min-width:200px">A-1 Mã yêu cầu</div><div class="fw-semibold flex-grow-1">${req.request_id}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted" style="min-width:200px">A-2 Người yêu cầu</div><div class="fw-semibold flex-grow-1">${req.requester_name} (${req.requester_code})</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted" style="min-width:200px">A-3 Chức vụ</div><div class="fw-semibold flex-grow-1">${req.requester_position}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted" style="min-width:200px">A-4 Phòng ban</div><div class="fw-semibold flex-grow-1">${req.dept}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted" style="min-width:200px">A-5 Ngày yêu cầu</div><div class="fw-semibold flex-grow-1">${req.request_date}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted" style="min-width:200px">A-6 Hạn hoàn thành</div><div class="fw-semibold flex-grow-1">${req.deadline}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted" style="min-width:200px">A-7 Tiêu đề</div><div class="fw-semibold flex-grow-1">${req.title}</div></div>
            <div class="d-flex py-2"><div class="text-muted" style="min-width:200px">A-8 Nội dung</div><div class="fw-semibold flex-grow-1">${req.content}</div></div>
        </div>

        <!-- Section B: Equipment Details -->
        ${req.equipment && req.equipment.length > 0 ? `
        <div class="bg-light-primary border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
            B. Chi tiết thiết bị yêu cầu
        </div>
        <div class="table-responsive mb-5">
            <table class="table table-sm table-row-bordered align-middle">
                <thead>
                    <tr class="fw-bold text-muted bg-light">
                        <th class="text-center">STT</th><th>Mã NV</th><th>Họ tên</th><th>Thiết bị</th><th>Mục đích</th>
                    </tr>
                </thead>
                <tbody>
                    ${req.equipment.map(e => `
                    <tr>
                        <td class="text-center">${e.no}</td>
                        <td>${e.nv_id}</td>
                        <td>${e.name}</td>
                        <td>${e.device}</td>
                        <td>${e.purpose}</td>
                    </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>` : ''}

        <!-- Section C: Attachments -->
        <div class="bg-light-primary border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
            C. File đính kèm
        </div>
        <div class="mb-5">
            ${req.attachments && req.attachments.length > 0 ? req.attachments.map(a => `<span class="badge badge-light-primary me-2 mb-2">${a}</span>`).join('') : '<div class="text-muted">Không có file đính kèm</div>'}
        </div>

        ${req.status === 'Rejected' ? `
        <div class="alert alert-danger d-flex align-items-center mb-0">
            <i class="ki-duotone ki-shield-cross fs-2x text-danger me-3"><span class="path1"></span><span class="path2"></span></i>
            <div><strong>Lý do từ chối:</strong> ${req.reject_reason || 'Không có lý do'}</div>
        </div>` : ''}
    `;

    $('#request_detail_container').html(html);
    $('#progress_container').removeClass('d-none');
    updateProgress(req.step);
};

// ==== Update Progress ====
const updateProgress = (step) => {
    $('[data-step-icon]').each(function() {
        const s = parseInt($(this).data('step-icon'), 10);
        $(this).removeClass('bg-light text-muted bg-success text-white bg-primary');
        if (s < step) {
            $(this).addClass('bg-success text-white');
        } else if (s === step) {
            $(this).addClass('bg-primary text-white');
        } else {
            $(this).addClass('bg-light text-muted');
        }
    });
};

// ==== Filter Counts ====
const updateFilterCounts = () => {
    $('#count_all').text(requestData.length);
    $('#count_accept').text(requestData.filter(r => r.status === 'Request').length);
    $('#count_approved').text(requestData.filter(r => r.status === 'Approved').length);
    $('#count_ongoing').text(requestData.filter(r => r.status === 'Ongoing').length);
    $('#count_confirm').text(requestData.filter(r => r.status === 'Confirm').length);
    $('#count_closed').text(requestData.filter(r => r.status === 'Done').length);
    $('#count_rejected').text(requestData.filter(r => r.status === 'Rejected').length);
};

// ==== Events ====
const bindEvents = () => {
    // Filter tabs
    $('.filter-tab').on('click', function() {
        $('.filter-tab').removeClass('active');
        $(this).addClass('active');
        currentFilter = $(this).data('filter');
        renderRequestList();
    });

    // Click request item
    $(document).on('click', '[data-id]', function(e) {
        if ($(e.target).closest('button').length) return; // ignore button clicks
        const id = $(this).data('id');
        if (id) renderRequestDetail(id);
    });

    // Search
    $('#txt_search_request').on('keyup', debounce(function() {
        const term = $(this).val().toLowerCase();
        $('#request_list_container > div').each(function() {
            const text = $(this).text().toLowerCase();
            $(this).toggle(text.includes(term));
        });
    }, 300));

    // New request
    $('#btn_new_request').on('click', () => openRequestForm());

    // Action buttons
    $('#btn_approve').on('click', () => approveRequest());
    $('#btn_delete').on('click', () => deleteRequest());
    $('#btn_save_request').on('click', () => saveRequest());

    // Equipment table
    $('#btn_add_equipment').on('click', () => addEquipmentRow());
};

// ==== Form Modal ====
const openRequestForm = (requestId = null) => {
    const req = requestId ? requestData.find(r => r.request_id === requestId) : null;
    
    $('#modal_form_title').text(req ? 'Chỉnh sửa yêu cầu' : 'Tạo yêu cầu mới');
    $('#form_request_no').val(req ? req.request_id : genRequestId());
    $('#form_receive_dept').val(req?.receive_dept || 'IT');
    $('#form_title').val(req?.title || 'Yêu cầu cấp thiết bị');
    $('#form_pub_date').val(req?.request_date.replace(/\//g, '-') || moment().format('YYYY-MM-DD'));
    $('#form_deadline').val(req?.deadline.replace(/\//g, '-') || moment().add(3, 'days').format('YYYY-MM-DD'));
    $('#form_req_dept').val(req?.dept || 'IT');
    $('#form_incharge').val(req?.incharge_email || 'user@company.vn');
    $('#form_content').val(req?.content || 'Vui lòng mô tả chi tiết yêu cầu cấp thiết bị...');

    $('#tbl_equipment tbody').empty();
    if (req?.equipment && req.equipment.length > 0) {
        req.equipment.forEach(e => addEquipmentRow(e));
    } else {
        addEquipmentRow();
    }

    new bootstrap.Modal($('#modal_request_form')[0]).show();
};

const addEquipmentRow = (data = {}) => {
    const no = $('#tbl_equipment tbody tr').length + 1;
    const row = `<tr>
        <td class="text-center">${no}</td>
        <td><input type="text" class="form-control form-control-sm eq-nv" value="${data.nv_id || ''}" placeholder="MV210222"></td>
        <td><input type="text" class="form-control form-control-sm eq-name" value="${data.name || ''}" placeholder="Full name"></td>
        <td><input type="text" class="form-control form-control-sm eq-device" value="${data.device || ''}" placeholder="Device/MAC"></td>
        <td><input type="text" class="form-control form-control-sm eq-purpose" value="${data.purpose || ''}" placeholder="Purpose"></td>
        <td><button type="button" class="btn btn-sm btn-icon btn-light-danger btn-remove-eq"><i class="ki-duotone ki-trash fs-5"></i></button></td>
    </tr>`;
    $('#tbl_equipment tbody').append(row);
};

$(document).on('click', '.btn-remove-eq', function() {
    $(this).closest('tr').remove();
    // Re-number
    $('#tbl_equipment tbody tr').each(function(i) {
        $(this).find('td:first').text(i + 1);
    });
});

const saveRequest = () => {
    const equipment = [];
    $('#tbl_equipment tbody tr').each(function() {
        equipment.push({
            no: $(this).find('td:first').text(),
            nv_id: $(this).find('.eq-nv').val(),
            name: $(this).find('.eq-name').val(),
            device: $(this).find('.eq-device').val(),
            purpose: $(this).find('.eq-purpose').val()
        });
    });

    const newReq = {
        request_id: $('#form_request_no').val(),
        request_date: moment($('#form_pub_date').val()).format('YYYY/MM/DD'),
        deadline: moment($('#form_deadline').val()).format('YYYY/MM/DD'),
        requester_code: 'NV999',
        requester_name: 'Người dùng hiện tại',
        requester_position: 'Nhân viên',
        dept: $('#form_req_dept').val(),
        receive_dept: $('#form_receive_dept').val(),
        title: $('#form_title').val(),
        content: $('#form_content').val(),
        status: 'Request',
        step: 1,
        equipment,
        incharge_email: $('#form_incharge').val(),
        attachments: []
    };

    requestData.unshift(newReq);
    bootstrap.Modal.getInstance($('#modal_request_form')[0]).hide();
    renderRequestList();
    updateFilterCounts();
    toastr.success('Đã tạo yêu cầu thành công');
};

const approveRequest = () => {
    const req = requestData.find(r => r.request_id === currentRequestId);
    if (req) {
        req.status = 'Approved';
        req.step = 2;
        renderRequestDetail(currentRequestId);
        renderRequestList();
        updateFilterCounts();
        toastr.success('Yêu cầu đã được phê duyệt');
    }
};

const deleteRequest = () => {
    if (!confirm('Bạn có chắc chắn muốn xóa yêu cầu này?')) return;
    const idx = requestData.findIndex(r => r.request_id === currentRequestId);
    if (idx > -1) {
        requestData.splice(idx, 1);
        currentRequestId = null;
        $('#request_detail_container').html(`<div class="text-center text-muted py-10">
            <i class="ki-duotone ki-document fs-3x text-gray-400"><span class="path1"></span><span class="path2"></span></i>
            <p class="mt-3">Chọn một yêu cầu từ danh sách để xem chi tiết</p>
        </div>`);
        $('#progress_container').addClass('d-none');
        renderRequestList();
        updateFilterCounts();
        toastr.success('Đã xóa yêu cầu');
    }
};

// ==== Utils ====
const genRequestId = () => `REQ-${moment().format('YYYYMMDD')}-${Math.floor(Math.random() * 999).toString().padStart(3, '0')}`;
const debounce = (func, wait) => {
    let timeout;
    return function(...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
};
