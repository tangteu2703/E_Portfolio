let requestData = [];
let currentFilter = 'all';
let currentRequestId = null;
$(() => {
    initData();
    bindEvents();
    initSplitLayout();

    comboBox.setSelectOptions();
});

const initData = async () => {
    await loadDeviceRequestsFromAPI();
};

// Helper function to reload data without re-initializing everything
const loadDeviceRequestsFromAPI = async () => {
    try {
        var request = {
            department: $('#cbo_Department').val(),
            status: currentFilter,
            textSearch: $('#txt_search_request').val(),
        }

        await apiHelper.post(`/Device/Select-Device-Request`, request,
            function (res) {
                var data = Array.isArray(res.data) ? res.data : [];

                // Update detail view if currentRequestId exists, otherwise show empty state
                if (currentRequestId && data.some(r => r.request_id === currentRequestId)) {
                    selectRequestDetail(currentRequestId);
                } else if (data.length > 0) {
                    currentRequestId = data[0].request_id
                    selectRequestDetail(currentRequestId);
                } else {
                    renderEmptyState();
                }

                renderRequestList(data);
                if (currentFilter == 'all')
                    updateFilterCounts(data);
            },
            function (err) {
                console.error("Lỗi reload device requests:", err);
                toastr.warning('Không thể kết nối API, thử lại sau.');
            });
    } catch (error) {
        console.error('Error reloading device requests:', error);
    }
};

// Helper function to get auth token (implement based on your auth system)
const getAuthToken = () => {
    // In a real app, get token from localStorage, sessionStorage, or cookies
    // For demo purposes, return empty string
    return '';
};

// ==== Render Request List ====
const renderRequestList = (data) => {
    const filtered = data.filter(r => {
        if (currentFilter === 'all') return true;
        return r.status === currentFilter;
    });

    const html = filtered.map(r => {
        const statusClass = getStatusClass(r.status);
        const isActive = currentRequestId === r.request_id;
        return `
        <div class="px-4 py-3 border-bottom border-start border-3 cursor-pointer hover-light-primary transition ${isActive ? 'border-primary bg-light-primary' : 'border-transparent'} "
           onclick="selectRequestDetail('${r.request_id}')" data-active="${r.request_id}">
            <div class="d-flex justify-content-between align-items-start mb-2">
                <div class="flex-grow-1">
                    <div class="fw-bold text-gray-800 fs-7">${r.request_id} | ${r.request_date} |
                       <span class="text-primary fw-semibold fs-8">${r.requester_dept}</span>
                    </div>
                    
                </div>
                <span class="badge ${statusClass} badge-sm">${r.status}</span>
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

    if (filtered.length === 0) {
        $('#request_list_container').html(`
            <div class="text-center text-muted py-10">
                <i class="ki-duotone ki-search fs-3x text-gray-400 mb-3">
                    <span class="path1"></span><span class="path2"></span>
                </i>
                <h5 class="text-gray-600">Không tìm thấy yêu cầu nào</h5>
                <p class="text-gray-500">Thử thay đổi từ khóa tìm kiếm hoặc bộ lọc</p>
                <button class="btn btn-outline-primary btn-sm" onclick="currentFilter='all';loadDeviceRequestsFromAPI();">
                    <i class="ki-duotone ki-arrow-left fs-5 me-1"></i>Hiển thị tất cả
                </button>
            </div>
        `);
    } else {
        $('#request_list_container').html(html);
    }
};

const customActive = (requestId) => {
    // 1️⃣ Xóa class active ở tất cả các item
    $('[data-active]').removeClass('border-primary bg-light-primary').addClass('border-transparent');

    // 2️⃣ Thêm class active vào item được chọn
    $(`[data-active="${requestId}"]`)
        .removeClass('border-transparent')
        .addClass('border-primary bg-light-primary');
};

const getStatusClass = (status) => {
    const map = {
        'Request': 'badge-warning text-dark',
        'Warning': 'badge-warning text-dark',
        'Approved': 'badge-info',
        'Ongoing': 'badge-primary',
        'Confirm': 'badge-success',
        'Done': 'badge-dark',
        'Rejected': 'badge-danger',
        'E_COMPLETION': 'badge-primary'
    };
    return map[status] || 'secondary';
};

// ==== Render Detail ====
const selectRequestDetail = async (requestId) => {

    if (!requestId) {
        $('#request_detail_container').html(`<div class="text-center text-muted py-10">
            <i class="ki-duotone ki-document fs-3x text-gray-400 mb-3">
                <span class="path1"></span><span class="path2"></span>
            </i>
            <h4 class="text-gray-600">Chọn một yêu cầu để xem chi tiết</h4>
            <p class="text-gray-500">Click vào một yêu cầu từ danh sách để xem thông tin chi tiết</p>
        </div>`);
        $('#progress_container').addClass('d-none');
        return;
    }

    try {
        await apiHelper.get(`/Device/Select-Device-Request-Detail`, { request_code: requestId },
            function (res) {
                requestData = Array.isArray(res.data) ? res.data : [];
                renderRequestDetail(requestId);
            },
            function (err) {
                console.error("Lỗi reload device requests:", err);
                renderRequestDetailNull(requestId);
            });
    } catch (error) {
        console.error('Error reloading device requests:', error);
    }
}

const renderRequestDetailNull = (requestId) => {
    $('#request_detail_container').html(`<div class="text-center text-muted py-10">
                    <i class="ki-duotone ki-search fs-3x text-gray-400 mb-3">
                        <span class="path1"></span><span class="path2"></span>
                    </i>
                    <h4 class="text-gray-600">Không tìm thấy yêu cầu : ${requestId}</h4>
                    <p class="text-gray-500">Yêu cầu này có thể đã bị xóa hoặc không tồn tại</p>
                </div>`);
    $('#progress_container').addClass('d-none');
}

const renderRequestDetail = (requestId) => {

    var req = requestData.find(r => r.request_id === requestId);
    if (!req)
        renderRequestDetailNull(requestId);

    currentRequestId = requestId;
    customActive(requestId); //show active

    $('#detail_request_code').text(req.request_id + ' | ' + req.title);
    $('#detail_request_title').text(req.requester_dept);

    // Show/hide action buttons based on status and user permissions
    $('#btn_approve, #btn_edit, #btn_reject, #btn_delete, #btn_view').addClass('d-none');

    // Always show view and edit buttons when request is selected
    $('#btn_view, #btn_edit').removeClass('d-none');

    // Show approval/reject buttons only for pending requests and managers
    if ((req.status === 'Request' || req.status === 'Warning') && isManager()) {
        $('#btn_approve, #btn_reject').removeClass('d-none');
    }

    // Show delete button for own requests or when user has permission
    if (canDeleteRequest(req)) {
        $('#btn_delete').removeClass('d-none');
    }

    const html = `
        <!-- Section A: Request Information -->
        <div class="bg-light-primary border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
            A. Thông tin yêu cầu thiết bị
        </div>
        <div class="mb-5">
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">A.1 Mã yêu cầu</div><div class="fw-semibold flex-grow-1">${req.request_id}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">A.2 Người yêu cầu</div><div class="fw-semibold flex-grow-1">${req.requester_code} - ${req.requester_name}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">A.3 Chức vụ</div><div class="fw-semibold flex-grow-1">${req.requester_position}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">A.4 Phòng ban</div><div class="fw-semibold flex-grow-1">${req.requester_dept}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">A.5 Ngày yêu cầu</div><div class="fw-semibold flex-grow-1">${req.request_date}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">A.6 Hạn hoàn thành</div><div class="fw-semibold flex-grow-1">${req.deadline}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">A.7 Tiêu đề</div><div class="fw-semibold flex-grow-1">${req.title}</div></div>
            <div class="d-flex py-2"><div class="text-muted min-width-200">A.8 Nội dung</div><div class="fw-semibold flex-grow-1">${req.content}</div></div>
        </div>

        <!-- Section B: Department Approve -->
        <div class="bg-light-primary border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
            B. Trưởng phòng yêu cầu phê duyệt
        </div>
        <div class="mb-5">
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">B.1 Người phê duyệt</div><div class="fw-semibold flex-grow-1">${req.request_approve.approve_code} - ${req.request_approve.approve_name}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">B.2 Chức vụ</div><div class="fw-semibold flex-grow-1">${req.request_approve.approve_position}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">B.3 Trạng thái</div><div class="fw-semibold flex-grow-1"><span class="badge ${getStatusClass(req.request_approve.approve_status)} badge-sm">${req.request_approve.approve_status}</span></div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">B.4 Ngày phê duyệt</div><div class="fw-semibold flex-grow-1">${req.request_approve.approve_date || 'Chưa phê duyệt'}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">B.5 Nội dung phê duyệt</div><div class="fw-semibold flex-grow-1">${req.request_approve.approve_content || 'Chưa có nội dung phê duyệt'}</div></div>
        
            </div>

        <!-- Section C: IT Manager Approve -->
        ${req.request_approve.approve_status == 'Approved' ? `
        <div class="bg-light-primary border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
            C. Trưởng phòng bàn giao phê duyệt
        </div>
        <div class="mb-5">
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">C.1 Người phê duyệt</div><div class="fw-semibold flex-grow-1">${req.publish_approve.approve_code || 'Chưa phê duyệt'} - ${req.publish_approve.approve_name || ''}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">C.2 Chức vụ</div><div class="fw-semibold flex-grow-1">${req.publish_approve.approve_position || ''}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">C.3 Trạng thái</div><div class="fw-semibold flex-grow-1"><span class="badge ${getStatusClass(req.publish_approve.approve_status)} badge-sm">${req.publish_approve.approve_status || 'Chưa phê duyệt'}</span></div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">C.4 Ngày phê duyệt</div><div class="fw-semibold flex-grow-1">${req.publish_approve.approve_date || 'Chưa phê duyệt'}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">C.5 Nội dung phê duyệt</div><div class="fw-semibold flex-grow-1">${req.publish_approve.approve_content || 'Chưa có nội dung phê duyệt'}</div></div>
            </div` : ''}

        <!-- Section D: Equipment Details -->
        ${req.publish_approve.approve_status == 'Approved' ? `
        <div class="bg-light-primary border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
            D. Chi tiết thiết bị yêu cầu
        </div>
        <div class="mb-5">
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">D.1 Người thực hiện</div><div class="fw-semibold flex-grow-1">${req.publish_confirm.confirm_code || 'Chưa phân công'} - ${req.publish_confirm.confirm_name || ''}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">D.2 Chức vụ</div><div class="fw-semibold flex-grow-1">${req.publish_confirm.confirm_position || ''}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">D.3 Trạng thái thực hiện</div><div class="fw-semibold flex-grow-1"><span class="badge ${getStatusClass(req.publish_confirm.confirm_status)} badge-sm">${req.publish_confirm.confirm_status || 'Chưa thực hiện'}</span></div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">D.4 Ngày hoàn thành</div><div class="fw-semibold flex-grow-1">${req.publish_confirm.confirm_date || 'Chưa hoàn thành'}</div></div>
        </div>

        <div class="table-responsive mb-5">
            <table class="table table-sm table-row-bordered align-middle">
                <thead>
                    <tr class="fw-bold text-muted bg-light">
                        <th class="text-center">STT</th><th>Mã thiết bị</th><th>Tên thiết bị</th><th>Người sử dụng</th><th>Ghi chú</th>
                    </tr>
                </thead>
                <tbody>
                    ${req.equipment.map((e, index) => `
                    <tr>
                        <td class="text-center">${index + 1}</td>
                        <td>${e.device_code}</td>
                        <td>${e.device_name}</td>
                        <td>${e.user_code} - ${e.user_name}</td>
                        <td>${e.purpose}</td>
                    </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>

        <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">D.5 File đính kèm</div>
            <div class="fw-semibold flex-grow-1">
                ${req.attachments && req.attachments.length > 0 ? req.attachments.map(a => `<span class="badge bg-light text-primary me-2 mb-2">${a}</span>`).join('') : '<div class="text-muted">Không có file đính kèm</div>'}
            </div>
        </div>` : ''}

        <!-- Section E: Bàn giao ký nhận -->
        ${req.publish_confirm.confirm_status === 'Confirm' || req.status === 'Done' ? `
        <div class="bg-light border-start border-primary border-3 px-3 py-2 fw-bold mb-4">
           E. Ký nhận bàn giao thiết bị
        </div>
        <div class="mb-5">
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">E.1 Người nhận</div><div class="fw-semibold flex-grow-1">${req.requester_code} - ${req.requester_name}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">E.2 Chức vụ</div><div class="fw-semibold flex-grow-1">${req.requester_position}</div></div>
            <div class="d-flex py-2 border-bottom"><div class="text-muted min-width-200">E.3 Ngày nhận</div><div class="fw-semibold flex-grow-1">${req.request_date}</div></div>
        </div>
        
        <!-- Digital Signatures Display -->
        <div class="bg-light-warning border-start border-warning border-3 px-3 py-2 fw-bold mb-4">
           E.6 Chữ ký điện tử
        </div>
        <div class="row g-4 mb-5">
            <div class="col-md-6">
                <div class="card border border-primary">
                    <div class="card-header bg-light-primary py-2">
                        <h6 class="card-title mb-0 text-primary fw-bold">
                            <i class="ki-duotone ki-user-tick fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                            Người bàn giao
                        </h6>
                    </div>
                    <div class="card-body text-center">
                        ${req.deliverer_signature ? 
                            `<img src="${req.deliverer_signature}" alt="Chữ ký người bàn giao" class="img-fluid border rounded" style="max-height: 150px;">
                            <div class="mt-2 text-muted small">${req.publish_confirm.confirm_code || ''} - ${req.publish_confirm.confirm_name || ''}</div>
                            <div class="text-muted small">${req.publish_confirm.confirm_position || 'IT Staff'}</div>` 
                            : '<div class="text-muted py-4"><i class="ki-duotone ki-question fs-3x"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i><p class="mt-2">Chưa có chữ ký</p></div>'}
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card border border-success">
                    <div class="card-header bg-light-success py-2">
                        <h6 class="card-title mb-0 text-success fw-bold">
                            <i class="ki-duotone ki-profile-user fs-5 me-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span></i>
                            Người nhận bàn giao
                        </h6>
                    </div>
                    <div class="card-body text-center">
                        ${req.receiver_signature ? 
                            `<img src="${req.receiver_signature}" alt="Chữ ký người nhận" class="img-fluid border rounded" style="max-height: 150px;">
                            <div class="mt-2 text-muted small">${req.requester_code} - ${req.requester_name}</div>
                            <div class="text-muted small">${req.requester_position}</div>` 
                            : '<div class="text-muted py-4"><i class="ki-duotone ki-question fs-3x"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i><p class="mt-2">Chưa có chữ ký</p></div>'}
                    </div>
                </div>
            </div>
        </div>` : ''}

        ${req.status === 'Rejected' ? `
        <div class="alert alert-danger d-flex align-items-center mb-0">
            <i class="ki-duotone ki-shield-cross fs-2x text-danger me-3"><span class="path1"></span><span class="path2"></span></i>
            <div><strong>Lý do từ chối:</strong> ${req.reject_reason || 'Không có lý do được ghi nhận'}</div>
        </div>` : ''}
    `;

    $('#request_detail_container').html(html);
    $('#progress_container').removeClass('d-none');
    updateProgress(req.step);
};

// ==== Update Progress ====
const updateProgress = (step) => {
    $('[data-step-icon]').each(function () {
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
const updateFilterCounts = (data) => {

    if (!data || data.length === 0) {
        $('#count_all, #count_accept, #count_approved, #count_ongoing, #count_confirm, #count_closed, #count_rejected')
            .text('0');
        return;
    }

    $('#count_all').text(data.length);
    $('#count_accept').text(data.filter(r => r.status === 'Request').length);
    $('#count_approved').text(data.filter(r => r.status === 'Approved').length);
    $('#count_ongoing').text(data.filter(r => r.status === 'Ongoing').length);
    $('#count_confirm').text(data.filter(r => r.status === 'Confirm').length);
    $('#count_closed').text(data.filter(r => r.status === 'Done').length);
    $('#count_rejected').text(data.filter(r => r.status === 'Rejected').length);
};

// ==== Empty State ====
const renderEmptyState = () => {
    $('#request_list_container').html(`
        <div class="text-center text-muted py-10">
            <i class="ki-duotone ki-document fs-3x text-gray-400 mb-3">
                <span class="path1"></span><span class="path2"></span>
            </i>
            <h4 class="text-gray-600">Không có yêu cầu nào</h4>
            <p class="text-gray-500 mb-4">Chưa có yêu cầu thiết bị nào được tạo</p>
            <button class="btn btn-primary" id="btn_create_first_request">
                <i class="ki-duotone ki-plus fs-4 me-2"></i>Tạo yêu cầu đầu tiên
            </button>
        </div>
    `);

    $('#request_detail_container').html(`
        <div class="text-center text-muted py-10">
            <i class="ki-duotone ki-document fs-3x text-gray-400 mb-3">
                <span class="path1"></span><span class="path2"></span>
            </i>
            <h4 class="text-gray-600">Chọn một yêu cầu để xem chi tiết</h4>
            <p class="text-gray-500">Click vào một yêu cầu từ danh sách để xem thông tin chi tiết</p>
        </div>
    `);

    $('#progress_container').addClass('d-none');

    // Bind event for create first request button
    $('#btn_create_first_request').on('click', () => openRequestForm());
};

// ==== Events ====
const bindEvents = () => {
    // Filter tabs
    $('.filter-tab').on('click', async function () {
        $('.filter-tab').removeClass('active');
        $(this).addClass('active');
        currentFilter = $(this).data('filter');
        console.log(currentFilter);
        await loadDeviceRequestsFromAPI();

    });

    // Search
    $('#txt_search_request').on('keyup', debounce(async function () {
        const term = $(this).val().toLowerCase();

        if (term.length >= 2) {
            try {
                // Call search API
                await apiHelper.get(`/Device/Search-Requests?term=${encodeURIComponent(term)}`,
                    {},
                    function (res) {
                        requestData = Array.isArray(res.data) ? res.data : [];
                        renderRequestList();
                        updateFilterCounts();
                    },
                    function (err) {
                        console.error('Error searching requests:', err);
                        // Fallback to local search
                        if (Array.isArray(requestData)) {
                            const searchData = requestData.filter(r =>
                                r.title.toLowerCase().includes(term) ||
                                r.requester_name.toLowerCase().includes(term) ||
                                r.requester_dept.toLowerCase().includes(term) ||
                                r.request_id.toLowerCase().includes(term)
                            );
                            requestData = searchData;
                        }
                        renderRequestList();
                        updateFilterCounts();
                    });
            } catch (error) {
                console.error('Error searching requests:', error);
                // Fallback to local search
                if (Array.isArray(requestData)) {
                    const searchData = requestData.filter(r =>
                        r.title.toLowerCase().includes(term) ||
                        r.requester_name.toLowerCase().includes(term) ||
                        r.requester_dept.toLowerCase().includes(term) ||
                        r.request_id.toLowerCase().includes(term)
                    );
                    requestData = searchData;
                }
                renderRequestList();
                updateFilterCounts();
            }
        } else if (term.length === 0) {
            // Reload all data when search is cleared
            try {
                await loadDeviceRequests();
            } catch (error) {
                console.error('Error reloading requests:', error);
                // Fallback to fake data if API fails
                requestData = Array.isArray(generateFakeData()) ? generateFakeData() : [];
                renderRequestList();
                updateFilterCounts();
            }
        }
    }, 300));

    // New request
    $('#btn_new_request').on('click', () => openRequestForm());

    // Action buttons
    $('#btn_approve').on('click', () => approveRequest());
    $('#btn_reject').on('click', () => rejectRequest());
    $('#btn_view').on('click', () => editRequest());
    $('#btn_delete').on('click', () => deleteRequest());
    $('#btn_save_request').on('click', () => saveRequest());

    // Equipment table
    $('#btn_add_equipment').on('click', () => addEquipmentRow());
};

// ==== Form Modal ====
const openRequestForm = (requestId = null) => {
    const req = requestId ? requestData.find(r => r.request_id === requestId) : null;

    $('#modal_form_title').text(req ? 'Thông tin chi tiết' : 'Tạo yêu cầu mới');
    
    // Section A: Request Info
    $('#form_request_no').val(req ? req.request_id : genRequestId());
    $('#form_receive_dept').val(req?.receive_dept || 'IT');
    $('#form_requester_name').val(req?.requester_name || 'NV001 - Nguyễn Văn A');
    $('#form_requester_position').val(req?.requester_position || 'Nhân viên');
    $('#form_title').val(req?.title || 'Yêu cầu cấp thiết bị');
    $('#form_pub_date').val(req?.request_date?.replace(/\//g, '-') || moment().format('YYYY-MM-DD'));
    $('#form_deadline').val(req?.deadline?.replace(/\//g, '-') || moment().add(3, 'days').format('YYYY-MM-DD'));
    $('#form_req_dept').val(req?.requester_dept || 'IT');
    $('#form_incharge').val(req?.requester_email || 'user@company.vn');
    $('#form_content').val(req?.content || 'Vui lòng mô tả chi tiết yêu cầu cấp thiết bị...');

    // Section B: Department Approve
    if (req?.request_approve) {
        $('#form_dept_approve_code').val(req.request_approve.approve_code || '');
        $('#form_dept_approve_position').val(req.request_approve.approve_position || '');
        $('#form_dept_approve_status').val(req.request_approve.approve_status || 'Request');
        $('#form_dept_approve_date').val(req.request_approve.approve_date?.replace(/\//g, '-') || '');
        $('#form_dept_approve_content').val(req.request_approve.approve_content || '');
    } else {
        $('#form_dept_approve_code, #form_dept_approve_position, #form_dept_approve_date, #form_dept_approve_content').val('');
        $('#form_dept_approve_status').val('Request');
    }

    // Section C: IT Manager Approve
    if (req?.publish_approve) {
        $('#form_it_approve_code').val(req.publish_approve.approve_code || '');
        $('#form_it_approve_position').val(req.publish_approve.approve_position || '');
        $('#form_it_approve_status').val(req.publish_approve.approve_status || 'Request');
        $('#form_it_approve_date').val(req.publish_approve.approve_date?.replace(/\//g, '-') || '');
        $('#form_it_approve_content').val(req.publish_approve.approve_content || '');
    } else {
        $('#form_it_approve_code, #form_it_approve_position, #form_it_approve_date, #form_it_approve_content').val('');
        $('#form_it_approve_status').val('Request');
    }

    // Section D: IT Confirmation & Equipment
    if (req?.publish_confirm) {
        $('#form_it_confirm_code').val(req.publish_confirm.confirm_code || '');
        $('#form_it_confirm_position').val(req.publish_confirm.confirm_position || '');
        $('#form_it_confirm_status').val(req.publish_confirm.confirm_status || 'Ongoing');
        $('#form_it_confirm_date').val(req.publish_confirm.confirm_date?.replace(/\//g, '-') || '');
    } else {
        $('#form_it_confirm_code, #form_it_confirm_position, #form_it_confirm_date').val('');
        $('#form_it_confirm_status').val('Ongoing');
    }

    $('#tbl_equipment tbody').empty();
    if (req?.equipment && req.equipment.length > 0) {
        req.equipment.forEach(e => addEquipmentRow(e));
    } else {
        addEquipmentRow();
    }

    // Section E: Receiver Confirmation
    if (req && (req.status === 'Confirm' || req.status === 'Done')) {
        $('#form_receiver_code').val(req.requester_code || '');
        $('#form_receiver_position').val(req.requester_position || '');
        $('#form_receiver_date').val(req.request_date?.replace(/\//g, '-') || '');
        $('#form_receiver_status').val(req.status || 'Confirm');
        $('#form_receiver_note').val('');
    } else {
        $('#form_receiver_code, #form_receiver_position, #form_receiver_date, #form_receiver_note').val('');
        $('#form_receiver_status').val('Confirm');
    }

    // Determine which progress tab to show based on request status
    let progressStep = 'A';
    
    if (req) {
        progressStep = getProgressStepByStatus(req);
    }
    
    // Initialize to the appropriate tab
    switchModalProgress(progressStep);

    // Show modal first
    const modal = new bootstrap.Modal($('#modal_request_form')[0]);
    modal.show();

    // Enable/disable tabs based on progress
    // If creating new request, enable all tabs for input
    if (req) {
        enableProgressTabs(progressStep);
    } else {
        enableAllProgressTabs();
    }

    // Initialize signature canvases after modal is shown
    setTimeout(() => {
        initSignatureCanvas('deliverer');
        initSignatureCanvas('receiver');
        loadSignatureTemplates('deliverer');
        loadSignatureTemplates('receiver');
        
        // Update deliverer info from IT confirmation
        updateDelivererInfo();
        
        // Load existing signatures if available
        if (req?.deliverer_signature) {
            loadSignatureImage('deliverer', req.deliverer_signature);
        }
        if (req?.receiver_signature) {
            loadSignatureImage('receiver', req.receiver_signature);
        }
    }, 300);
};

const loadSignatureImage = (type, imageData) => {
    const canvas = signatureCanvases[type]?.canvas;
    const ctx = signatureCanvases[type]?.ctx;
    if (!canvas || !ctx || !imageData) return;

    const img = new Image();
    img.onload = function() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(img, 0, 0);
    };
    img.src = imageData;
};

const addEquipmentRow = (data = {}) => {
    const no = $('#tbl_equipment tbody tr').length + 1;
    const row = `<tr>
        <td class="text-center">${no}</td>
        <td><input type="text" class="form-control form-control-sm eq-device-code" value="${data.device_code || ''}" placeholder="PC-001"></td>
        <td><input type="text" class="form-control form-control-sm eq-device-name" value="${data.device_name || ''}" placeholder="PC Desktop Dell"></td>
        <td><input type="text" class="form-control form-control-sm eq-user-code" value="${data.user_code || ''}" placeholder="NV001"></td>
        <td><input type="text" class="form-control form-control-sm eq-user-name" value="${data.user_name || ''}" placeholder="NV001 - Nguyễn Văn A"></td>
        <td><input type="text" class="form-control form-control-sm eq-purpose" value="${data.purpose || ''}" placeholder="Ghi chú"></td>
        <td><button type="button" class="btn btn-sm btn-icon btn-light-danger btn-remove-eq"><i class="ki-duotone ki-trash fs-5">
            <span class="path1"></span>
            <span class="path2"></span>
            <span class="path3"></span>
            <span class="path4"></span>
            <span class="path5"></span></i>
            </button>
         </td>
    </tr>`;
    $('#tbl_equipment tbody').append(row);
};

// ==== Modal Progress Tab Switching ====
const switchModalProgress = (step) => {
    // Hide all progress content sections
    $('.progress-content').addClass('d-none');
    
    // Show the selected progress content
    $('#progress_' + step).removeClass('d-none');
    
    // Update the visual state of progress tabs
    $('[data-modal-step-icon]').each(function() {
        const icon = $(this).data('modal-step-icon');
        const parent = $(this).parent();
        
        if (icon === step) {
            // Active step - highlight
            $(this).removeClass('bg-light text-muted bg-success').addClass('bg-primary text-white');
            parent.find('.small').first().removeClass('text-muted').addClass('fw-bold');
        } else {
            // Inactive step - default
            $(this).removeClass('bg-primary text-white').addClass('bg-light text-muted');
            parent.find('.small').first().removeClass('fw-bold').addClass('text-muted');
        }
    });
};

// Get progress step based on request status
const getProgressStepByStatus = (req) => {
    if (!req) return 'A';
    
    const status = req.status;
    
    // Map status to progress steps
    if (status === 'Request' || status === 'Warning') {
        return 'B'; // Waiting for department approval
    } else if (status === 'Approved') {
        return 'C'; // Department approved, waiting for IT approval
    } else if (status === 'Ongoing') {
        return 'D'; // IT approved, working on equipment
    } else if (status === 'Confirm') {
        return 'E'; // Equipment delivered, waiting for confirmation
    } else if (status === 'Done') {
        return 'E'; // Completed
    } else if (status === 'Rejected') {
        return 'B'; // Rejected at approval stage
    }
    
    return 'A'; // Default to first step
};

// Enable/disable progress tabs based on current status
const enableProgressTabs = (currentStep) => {
    const stepOrder = ['A', 'B', 'C', 'D', 'E'];
    const currentIndex = stepOrder.indexOf(currentStep);
    
    stepOrder.forEach((step, index) => {
        const stepElement = $(`.progress-step[data-progress="${step}"]`);
        const icon = $(`[data-modal-step-icon="${step}"]`);
        
        // Reset pointer events and opacity first
        stepElement.css('pointer-events', '');
        
        if (index < currentIndex) {
            // Past steps - completed (green) and clickable
            icon.removeClass('bg-light text-muted bg-primary').addClass('bg-success text-white');
            stepElement.removeClass('disabled-tab').addClass('clickable-tab');
            stepElement.css('opacity', '1');
        } else if (index === currentIndex) {
            // Current step - active (blue) and clickable
            icon.removeClass('bg-light text-muted bg-success').addClass('bg-primary text-white');
            stepElement.removeClass('disabled-tab').addClass('clickable-tab');
            stepElement.css('opacity', '1');
            stepElement.find('.small').first().addClass('fw-bold').removeClass('text-muted');
        } else {
            // Future steps - disabled (gray, semi-transparent)
            icon.removeClass('bg-primary text-white bg-success').addClass('bg-light text-muted');
            stepElement.addClass('disabled-tab').removeClass('clickable-tab');
            stepElement.css('opacity', '0.5');
            stepElement.css('pointer-events', 'none');
            stepElement.find('.small').removeClass('fw-bold').addClass('text-muted');
        }
    });
};

// Enable all progress tabs (for creating new request)
const enableAllProgressTabs = () => {
    const stepOrder = ['A', 'B', 'C', 'D', 'E'];
    
    stepOrder.forEach((step) => {
        const stepElement = $(`.progress-step[data-progress="${step}"]`);
        const icon = $(`[data-modal-step-icon="${step}"]`);
        
        // Enable all tabs
        icon.removeClass('bg-primary text-white bg-success').addClass('bg-light text-muted');
        stepElement.removeClass('disabled-tab').addClass('clickable-tab');
        stepElement.css('opacity', '1');
        stepElement.css('pointer-events', '');
    });
    
    // Set first tab as active
    const firstIcon = $(`[data-modal-step-icon="A"]`);
    firstIcon.removeClass('bg-light text-muted').addClass('bg-primary text-white');
    $(`.progress-step[data-progress="A"]`).find('.small').first().addClass('fw-bold');
};

// Handle progress tab click with validation
const handleProgressClick = (step) => {
    const stepElement = $(`.progress-step[data-progress="${step}"]`);
    
    // Check if this tab is disabled
    if (stepElement.hasClass('disabled-tab')) {
        toastr.warning('Bước này chưa thể truy cập. Vui lòng hoàn thành các bước trước.');
        return;
    }
    
    // Switch to the clicked tab
    switchModalProgress(step);
};

// Make functions globally accessible for onclick handlers
window.switchModalProgress = switchModalProgress;
window.handleProgressClick = handleProgressClick;

// ==== Digital Signature Canvas Handler ====
let signatureCanvases = {
    deliverer: null,
    receiver: null
};

let isDrawing = {
    deliverer: false,
    receiver: false
};

const initSignatureCanvas = (type) => {
    const canvas = document.getElementById(`canvas_${type}_signature`);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    signatureCanvases[type] = { canvas, ctx };

    // Set canvas background
    ctx.fillStyle = '#ffffff';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Set drawing style
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 2;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';

    // Mouse events
    canvas.addEventListener('mousedown', (e) => startDrawing(e, type));
    canvas.addEventListener('mousemove', (e) => draw(e, type));
    canvas.addEventListener('mouseup', () => stopDrawing(type));
    canvas.addEventListener('mouseout', () => stopDrawing(type));

    // Touch events for mobile
    canvas.addEventListener('touchstart', (e) => {
        e.preventDefault();
        const touch = e.touches[0];
        const mouseEvent = new MouseEvent('mousedown', {
            clientX: touch.clientX,
            clientY: touch.clientY
        });
        canvas.dispatchEvent(mouseEvent);
    });

    canvas.addEventListener('touchmove', (e) => {
        e.preventDefault();
        const touch = e.touches[0];
        const mouseEvent = new MouseEvent('mousemove', {
            clientX: touch.clientX,
            clientY: touch.clientY
        });
        canvas.dispatchEvent(mouseEvent);
    });

    canvas.addEventListener('touchend', (e) => {
        e.preventDefault();
        const mouseEvent = new MouseEvent('mouseup', {});
        canvas.dispatchEvent(mouseEvent);
    });
};

const startDrawing = (e, type) => {
    isDrawing[type] = true;
    const canvas = signatureCanvases[type].canvas;
    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    const ctx = signatureCanvases[type].ctx;
    ctx.beginPath();
    ctx.moveTo(x, y);
};

const draw = (e, type) => {
    if (!isDrawing[type]) return;

    const canvas = signatureCanvases[type].canvas;
    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    const ctx = signatureCanvases[type].ctx;
    ctx.lineTo(x, y);
    ctx.stroke();
};

const stopDrawing = (type) => {
    isDrawing[type] = false;
};

const clearSignature = (type) => {
    const canvas = signatureCanvases[type]?.canvas;
    const ctx = signatureCanvases[type]?.ctx;
    if (!canvas || !ctx) return;

    ctx.fillStyle = '#ffffff';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    
    // Clear the select dropdown
    $(`#form_${type}_signature_select`).val('');
};

const saveSignatureAsTemplate = (type) => {
    const canvas = signatureCanvases[type]?.canvas;
    if (!canvas) return;

    // Check if canvas is empty
    const imageData = canvas.toDataURL('image/png');
    
    // Save to localStorage
    const signatures = JSON.parse(localStorage.getItem(`signatures_${type}`) || '[]');
    const timestamp = new Date().toISOString();
    signatures.push({
        id: `sig_${Date.now()}`,
        name: `Chữ ký ${signatures.length + 1} - ${new Date().toLocaleDateString('vi-VN')}`,
        data: imageData,
        timestamp: timestamp
    });
    
    localStorage.setItem(`signatures_${type}`, JSON.stringify(signatures));
    
    // Reload signature options
    loadSignatureTemplates(type);
    
    toastr.success(`Đã lưu chữ ký mẫu thành công`);
};

const loadSignatureTemplates = (type) => {
    const signatures = JSON.parse(localStorage.getItem(`signatures_${type}`) || '[]');
    const select = $(`#form_${type}_signature_select`);
    
    // Clear existing options except first one
    select.find('option:not(:first)').remove();
    
    // Add saved signatures
    signatures.forEach(sig => {
        select.append(`<option value="${sig.id}">${sig.name}</option>`);
    });
};

const loadSignatureFromTemplate = (type, signatureId) => {
    if (!signatureId) {
        clearSignature(type);
        return;
    }

    const signatures = JSON.parse(localStorage.getItem(`signatures_${type}`) || '[]');
    const signature = signatures.find(sig => sig.id === signatureId);
    
    if (!signature) return;

    const canvas = signatureCanvases[type]?.canvas;
    const ctx = signatureCanvases[type]?.ctx;
    if (!canvas || !ctx) return;

    const img = new Image();
    img.onload = function() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(img, 0, 0);
    };
    img.src = signature.data;
};

const getSignatureData = (type) => {
    const canvas = signatureCanvases[type]?.canvas;
    if (!canvas) return null;
    
    return canvas.toDataURL('image/png');
};

// Update deliverer info when IT confirmation info changes
const updateDelivererInfo = () => {
    const itCode = $('#form_it_confirm_code').val();
    const itPosition = $('#form_it_confirm_position').val();
    
    if (itCode) {
        $('#form_deliverer_info').val(`${itCode} - ${itPosition || 'IT Staff'}`);
    } else {
        $('#form_deliverer_info').val('');
    }
};

// Event handlers for signature select dropdowns
$(document).on('change', '#form_deliverer_signature_select', function() {
    loadSignatureFromTemplate('deliverer', $(this).val());
});

$(document).on('change', '#form_receiver_signature_select', function() {
    loadSignatureFromTemplate('receiver', $(this).val());
});

// Watch for changes in IT confirmation fields
$(document).on('input', '#form_it_confirm_code, #form_it_confirm_position', function() {
    updateDelivererInfo();
});

// Make functions globally accessible
window.clearSignature = clearSignature;
window.saveSignatureAsTemplate = saveSignatureAsTemplate;

$(document).on('click', '.btn-remove-eq', function () {
    $(this).closest('tr').remove();
    // Re-number
    $('#tbl_equipment tbody tr').each(function (i) {
        $(this).find('td:first').text(i + 1);
    });
});

const saveRequest = async () => {
    try {
        const equipment = [];
        $('#tbl_equipment tbody tr').each(function () {
            equipment.push({
                serial_id: parseInt($(this).find('td:first').text()),
                device_code: $(this).find('.eq-device-code').val(),
                device_name: $(this).find('.eq-device-name').val(),
                user_code: $(this).find('.eq-user-code').val(),
                user_name: $(this).find('.eq-user-name').val(),
                purpose: $(this).find('.eq-purpose').val()
            });
        });

        // Get signature data
        const delivererSignature = getSignatureData('deliverer');
        const receiverSignature = getSignatureData('receiver');

        const newRequest = {
            request_date: moment($('#form_pub_date').val()).format('YYYY/MM/DD'),
            deadline: moment($('#form_deadline').val()).format('YYYY/MM/DD'),
            requester_code: $('#form_receiver_code').val() || 'NV999',
            requester_name: $('#form_requester_name').val() || 'Người dùng hiện tại',
            requester_email: $('#form_incharge').val(),
            requester_position: $('#form_requester_position').val() || 'Nhân viên',
            requester_dept: $('#form_req_dept').val(),
            receive_dept: $('#form_receive_dept').val(),
            title: $('#form_title').val(),
            content: $('#form_content').val(),
            equipment: equipment,
            attachments: [],
            // Signature data
            deliverer_signature: delivererSignature,
            receiver_signature: receiverSignature,
            // Additional approval data
            dept_approve_status: $('#form_dept_approve_status').val(),
            dept_approve_content: $('#form_dept_approve_content').val(),
            it_approve_status: $('#form_it_approve_status').val(),
            it_approve_content: $('#form_it_approve_content').val(),
            it_confirm_status: $('#form_it_confirm_status').val(),
            receiver_status: $('#form_receiver_status').val(),
            receiver_note: $('#form_receiver_note').val()
        };

        await apiHelper.post('/Device/Create-Request',
            newRequest,
            function (res) {
                // Reload data from API
                loadDeviceRequestsFromAPI();
                bootstrap.Modal.getInstance($('#modal_request_form')[0]).hide();

                // Auto-select the newly created request
                if (res.data && res.data.request_id) {
                    setTimeout(() => {
                        renderRequestDetail(res.data.request_id);
                    }, 500);
                }

                toastr.success('Đã tạo yêu cầu thành công');
            },
            function (err) {
                console.error('Error creating request:', err);
                toastr.error('Không thể tạo yêu cầu mới');
            });
    } catch (error) {
        console.error('Error creating request:', error);
        toastr.error('Không thể tạo yêu cầu mới');
    }
};

const approveRequest = async () => {
    try {
        await apiHelper.post(`/Device/Approve-Request/${currentRequestId}`,
            {},
            function (res) {
                // Reload data from API
                loadDeviceRequestsFromAPI();
                toastr.success('Yêu cầu đã được phê duyệt');
            },
            function (err) {
                console.error('Error approving request:', err);
                toastr.error('Không thể phê duyệt yêu cầu');
                // Revert optimistic update by reloading data
                loadDeviceRequestsFromAPI();
            });
    } catch (error) {
        console.error('Error approving request:', error);
        toastr.error('Không thể phê duyệt yêu cầu');
    }
};

const rejectRequest = async () => {
    const reason = prompt('Nhập lý do từ chối:');
    if (!reason) return;

    try {
        await apiHelper.post(`/Device/Reject-Request/${currentRequestId}`,
            { reason: reason },
            function (res) {
                // Reload data from API
                loadDeviceRequestsFromAPI();
                toastr.error('Yêu cầu đã bị từ chối');
            },
            function (err) {
                console.error('Error rejecting request:', err);
                toastr.error('Không thể từ chối yêu cầu');
                // Revert optimistic update by reloading data
                loadDeviceRequestsFromAPI();
            });
    } catch (error) {
        console.error('Error rejecting request:', error);
        toastr.error('Không thể từ chối yêu cầu');
    }
};

const editRequest = () => {
    // Open form in edit mode
    openRequestForm(currentRequestId);
};

const deleteRequest = async () => {
    if (!confirm('Bạn có chắc chắn muốn xóa yêu cầu này?')) return;

    try {
        await apiHelper.delete(`/Device/Delete-Request/${currentRequestId}`,
            {},
            function (res) {
                // Reload data from API
                loadDeviceRequestsFromAPI();
                currentRequestId = null;
                $('#progress_container').addClass('d-none');

                // Check if there are any requests left
                if (requestData.length === 0) {
                    renderEmptyState();
                } else {
                    $('#request_detail_container').html(`<div class="text-center text-muted py-10">
                        <i class="ki-duotone ki-document fs-3x text-gray-400 mb-3">
                            <span class="path1"></span><span class="path2"></span>
                        </i>
                        <h4 class="text-gray-600">Chọn một yêu cầu để xem chi tiết</h4>
                        <p class="text-gray-500">Click vào một yêu cầu từ danh sách để xem thông tin chi tiết</p>
        </div>`);
                }

                toastr.success('Đã xóa yêu cầu');
            },
            function (err) {
                console.error('Error deleting request:', err);
                toastr.error('Không thể xóa yêu cầu');
                // Revert optimistic update by reloading data
                loadDeviceRequestsFromAPI();
            });
    } catch (error) {
        console.error('Error deleting request:', error);
        toastr.error('Không thể xóa yêu cầu');
    }
};

// ==== Split Layout ====
let isResizing = false;
let startX = 0, startY = 0;
let startWidth = 0, startHeight = 0;

const initSplitLayout = () => {
    const container = $('#split_container');
    const leftPanel = $('#split_left');
    const rightPanel = $('#split_right');
    const resizer = $('#split_resizer');

    if (!container.length) return;

    resizer.on('mousedown', function (e) {
        isResizing = true;
        startX = e.clientX;
        startY = e.clientY;
        startWidth = leftPanel.outerWidth();
        startHeight = leftPanel.outerHeight();

        resizer.addClass('dragging');
        e.preventDefault();
    });

    $(document).on('mousemove', function (e) {
        if (!isResizing) return;

        const deltaX = e.clientX - startX;
        const deltaY = e.clientY - startY;

        if (window.innerWidth <= 768) {
            // Mobile: vertical resize
            const newHeight = startHeight + deltaY;
            const minHeight = 300;
            const maxHeight = container.outerHeight() - 300;

            if (newHeight >= minHeight && newHeight <= maxHeight) {
                leftPanel.css('height', newHeight + 'px');
                rightPanel.css('height', (container.outerHeight() - newHeight - resizer.outerHeight()) + 'px');
            }
        } else {
            // Desktop: horizontal resize
            const newWidth = startWidth + deltaX;
            const minWidth = 400;
            const maxWidth = container.outerWidth() - 400;

            if (newWidth >= minWidth && newWidth <= maxWidth) {
                leftPanel.css('width', newWidth + 'px');
                rightPanel.css('width', (container.outerWidth() - newWidth - resizer.outerWidth()) + 'px');
            }
        }
    });

    $(document).on('mouseup', function () {
        if (isResizing) {
            isResizing = false;
            resizer.removeClass('dragging');
        }
    });

    // Handle window resize - Bootstrap responsive
    $(window).on('resize', function () {
        if (window.innerWidth <= 768) {
            // Mobile: reset to vertical layout
            leftPanel.add(rightPanel).css({ width: '', height: '' });
            resizer.removeClass('dragging');
        } else {
            // Desktop: reset to horizontal layout
            leftPanel.add(rightPanel).css({ width: '', height: '' });
        }
    });
};

// ==== Utils ====
const genRequestId = () => `REQ-${moment().format('YYYYMMDD')}-${Math.floor(Math.random() * 999).toString().padStart(3, '0')}`;
const debounce = (func, wait) => {
    let timeout;
    return function (...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
};

// Permission helpers
const isManager = () => {
    // In real app, check user role/permission
    // For demo, assume current user is manager for IT requests
    return true;
};

const canDeleteRequest = (request) => {
    // User can delete their own requests or admin can delete any
    // For demo, allow deletion for non-completed requests
    return request.status !== 'Done' && request.status !== 'Confirm';
};
