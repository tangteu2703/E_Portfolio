$(document).ready(function () {
    init();
});

const init = () => {
    // Gọi API thực tế (để sau này sử dụng)
    apiHelper.get(`/Account/Select-Account-By-Code`,
        {},
        function (res) {
            if (res && res.data) {
                const user = res.data;
                console.log("User Data từ API:", user);

                displayUserProfile(user);
                populateFormData(user);
            } else {
                console.warn("Không tìm thấy dữ liệu người dùng từ API");
            }
        },
        function (err) {
            console.error("Lỗi lấy thông tin profile từ API:", err);
        });

    // Setup form submit event
    setupFormEvents();
};

// Hàm thiết lập các sự kiện cho form
const setupFormEvents = () => {
    // Xử lý form submit
    $('#kt_account_profile_details_form').on('submit', function(e) {
        e.preventDefault(); // Ngăn form submit mặc định
        saveChanges(); // Gọi hàm save changes tùy chỉnh
    });

    console.log("Đã thiết lập các sự kiện cho form");
};

// Hàm hiển thị thông tin người dùng (cho cả dữ liệu mẫu và API)
const displayUserProfile = (user) => {
    // Header thông tin cơ bản
    $(".user_fullname").text(user.full_name || "Chưa có tên");
    $(".user_account").text(user.user_code || "Chưa có mã");
    $(".user_email").text(user.email || "Chưa có email");

    // Avatar trong header
    if (user.avatar_url) {
        $(".user_avatar").attr("src", user.avatar_url);
    }

    // Thông tin chi tiết trong tab Profile Details
    $("#user_department").text(user.department || "Chưa có phòng ban");
    $("#user_position").text(user.position || "Chưa có chức vụ");
    $("#user_title").text(user.title || "Chưa có chức danh");
    $("#user_code").text(user.user_code || "Chưa có mã");
    $(".user_fullname").text(user.full_name || "Chưa có tên");
    $("#user_phone").text(user.phone_number || "Chưa có số điện thoại");
    $("#user_email").text(user.email || "Chưa có email");
    $("#user_address").text(user.address || "Chưa có địa chỉ");
    $("#user_birthdate").text(user.birth_date ? formatDate(user.birth_date) : "Chưa có ngày sinh");
    $("#user_gender").text(user.gender ? "Nam" : "Nữ");
};

// Hàm điền dữ liệu vào form Settings
const populateFormData = (user) => {
    // Điền dữ liệu vào các input field trong form - sử dụng jQuery gán value
    $("#txt_user_fullname").val(user.full_name || "").trigger('change');
    $("#txt_user_phone").val(user.phone_number || "").trigger('change');
    $("#txt_user_email").val(user.email || "").trigger('change');
    $("#txt_birth_date").val(user.birth_date || "").trigger('change');
    $("#txt_city").val(user.city || "").trigger('change');
    $("#txt_address").val(user.address || "").trigger('change');

    // Set radio button cho giới tính - sử dụng jQuery gán value
    if (user.gender === true || user.gender === "true" || user.gender === "Nam" || user.gender === "1") {
        $("#gender_male").prop('checked', true).trigger('change');
        $("#gender_female").prop('checked', false).trigger('change');
    } else {
        $("#gender_female").prop('checked', true).trigger('change');
        $("#gender_male").prop('checked', false).trigger('change');
    }

    // Hiển thị avatar trong form Settings
    if (user.avatar_url) {
        // Cập nhật background-image của container và wrapper
        $('.image-input[data-kt-image-input="true"]').css('background-image', `url(${user.avatar_url})`);
        $('.image-input-wrapper').css('background-image', `url(${user.avatar_url})`);
    }

    console.log("Đã điền dữ liệu vào form Settings:", user);
};

// Hàm lưu thay đổi thông tin người dùng
const saveChanges = () => {
    // Thu thập dữ liệu từ form
    const fullName = $("#txt_user_fullname").val();
    const phone = $("#txt_user_phone").val();
    const email = $("#txt_user_email").val();
    const birthDate = $("#txt_birth_date").val();
    const city = $("#txt_city").val();
    const address = $("#txt_address").val();

    // Giới tính từ radio buttons
    const gender = $("#gender_male").is(':checked') ? true : false;

    // File avatar (nếu có)
    const avatarFile = $('input[name="avatar"]')[0]?.files[0];

    // Validation cơ bản
    if (!fullName || !phone || !email) {
        toastr.error("Vui lòng điền đầy đủ thông tin bắt buộc!");
        return;
    }

    // Tạo object dữ liệu người dùng
    const userData = {
        email: email,
        full_name: fullName,
        phone: phone,
        avatar: avatarFile || null,
        card_color: '', 
        usercode: ''
    };

    // Hiển thị loading
    const submitBtn = $('#kt_account_profile_details_form button[type="submit"]');
    const originalText = submitBtn.text();
    submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2"></span>Đang lưu...');

    console.log("Đang gửi dữ liệu cập nhật:", userData);

    // Gọi API cập nhật thông tin
    apiHelper.post(`/Account/Update-Account`, userData,
        function (res) {
            console.log("Cập nhật thành công:", res);

            // Hiển thị thông báo thành công
            toastr.success("Cập nhật thông tin thành công!");

            // Reset trạng thái button
            submitBtn.prop('disabled', false).html(originalText);

            // Refresh dữ liệu hiển thị
            if (res && res.data) {
                displayUserProfile(res.data);
                populateFormData(res.data);
            }
        },
        function (err) {
            console.error("Lỗi cập nhật thông tin:", err);

            // Reset trạng thái button
            submitBtn.prop('disabled', false).html(originalText);

            // Hiển thị thông báo lỗi
            const errorMessage = err.responseJSON?.message || "Có lỗi xảy ra khi cập nhật thông tin!";
            toastr.error(errorMessage);
        }
    );
};

