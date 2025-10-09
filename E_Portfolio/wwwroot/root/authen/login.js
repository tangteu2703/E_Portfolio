let timerInterval = null;

$(document).ready(function () {
    // Xoá token cũ
    localStorage.removeItem('e_atoken');
    localStorage.removeItem('e_rtoken');

    setupOTPInputBehavior();

    const hideRecoveryFields = () => {
        $('#sendMailBtn').removeClass('d-none');
        $('#confirmChangeBtn').addClass('d-none');
        $('#codeGroup, #newPasswordGroup, #confirmPasswordGroup').addClass('d-none');
        clearOTPFields();
        clearInterval(timerInterval);
    };

    $('#forgotPasswordLink').on('click', function (e) {
        e.preventDefault();
        $('#loginForm').addClass('fade-out').removeClass('fade-in');

        setTimeout(() => {
            $('#loginForm').addClass('d-none');
            $('#forgotPasswordForm').removeClass('d-none').addClass('fade-in');
            hideRecoveryFields();
        }, 200);
    });

    $('#backToLoginBtn').on('click', function (e) {
        e.preventDefault();
        $('#forgotPasswordForm').addClass('fade-out').removeClass('fade-in');

        setTimeout(() => {
            $('#forgotPasswordForm').addClass('d-none');
            $('#loginForm').removeClass('d-none').addClass('fade-in');
            hideRecoveryFields();
        }, 200);
    });
});

const login = async () => {
    const emailOrPhone = $('#email').val().trim();
    const password = $('#password').val().trim();
    const errorMessage = $('#errorMessage');
    const loginBtn = $('#loginBtn')[0];

    if (!emailOrPhone)
        return commonSwal.showWarning("Email hoặc số điện thoại không được để trống!");
    if (!password)
        return commonSwal.showWarning("Mật khẩu không được để trống!");

    const data = {
        email: emailOrPhone,
        phone_number: emailOrPhone,
        password,
        is_ldap: false
    };

    // Ẩn lỗi và bật loading
    commonButton.hideElement(errorMessage[0]);
    commonButton.setButtonLoading(loginBtn, true);

    await apiHelper.post(`/Authen/login`, data,
        async (res) => {
            const response = res.data;

            localStorage.setItem('e_atoken', response.access_token || '');
            localStorage.setItem('e_rtoken', response.refresh_token || '');
            localStorage.setItem('e_language', 'vn');

            const user = response.user_info || {};
            const userData = {
                email: user.email || '',
                full_name: user.full_name || '',
                phone: user.phone || '',
                avatar: user.avatar || '',
                card_color: user.card_color || '',
                usercode: user.user_code || ''
            };

            // 🔹 Xóa dữ liệu cũ rồi mới lưu mới
            await commonIndexDB.clearStore('AuthorizeDB', 'user_information');
            await commonIndexDB.clearStore('AuthorizeDB', 'e_permissions');

            // 🔹 Lưu dữ liệu mới
            await commonIndexDB.saveToIndexedDB('AuthorizeDB', 'user_information', userData, 'usercode')
                .then(() => console.log("✅ userData lưu thành công"))
                .catch(err => console.error("❌", err));

            await commonIndexDB.saveToIndexedDB('AuthorizeDB', 'e_permissions', response.list_permissions, 'permissions')
                .then(() => console.log("✅ e_permissions lưu thành công"))
                .catch(err => console.error("❌", err));


            window.location.href = "/";

            commonButton.setButtonLoading(loginBtn, false);
        },
        (err) => {
            errorMessage.textContent = err?.responseJSON?.message || "Đăng nhập thất bại.";
            commonButton.showElement(errorMessage[0]);
            commonButton.setButtonLoading(loginBtn, false);
        }
    );

};

// Lấy mã OTP từ 6 ô input
function getOTP() {
    return $('.otp-input').map(function () {
        return $(this).val();
    }).get().join('');
}

// Clear các ô input
function clearOTPFields() {
    $('.otp-input').val('');
}

// Xử lý hành vi tự focus và backspace
function setupOTPInputBehavior() {
    $(document).on('input', '.otp-input', function () {
        const val = $(this).val().replace(/[^0-9]/g, '');
        $(this).val(val);
        if (val.length === 1) {
            $(this).next('.otp-input').focus();
        }
    });

    $(document).on('keydown', '.otp-input', function (e) {
        if (e.key === 'Backspace' && $(this).val() === '') {
            $(this).prev('.otp-input').focus();
        }
    });

    // Cho phép paste vào ô đầu
    $('.otp-input').first().on('paste', function (e) {
        const pasted = e.originalEvent.clipboardData.getData('text').replace(/\D/g, '');
        if (pasted.length === 6) {
            $('.otp-input').each((i, el) => $(el).val(pasted[i] || ''));
            e.preventDefault();
        }
    });
}
const sendOTP = async () => {
    const email = $('#forgotEmail').val().trim();
    if (!email)
        return commonSwal.showWarning("Email không được để trống!");

    const btnId = $('#sendMailBtn')[0];
    commonButton.setButtonLoading(btnId, true, "Send Mail");

    await apiHelper.get(`/Account/Send-OTP`, { email: email },
        function (res) {
            if (timerInterval) clearInterval(timerInterval);

            let timeLeft = res.countdown * 60;
            const updateCountdown = () => {
                if (timeLeft <= 0) {
                    $('#countdownTimer').text('Expired');
                    clearInterval(timerInterval);
                    return;
                }
                const m = Math.floor(timeLeft / 60);
                const s = timeLeft % 60;
                $('#countdownTimer').text(`${m}:${s.toString().padStart(2, '0')}`);
                timeLeft--;
            };

            updateCountdown();
            timerInterval = setInterval(updateCountdown, 1000);

            // Hiện các field cần thiết
            $('#sendMailBtn').addClass('d-none');
            $('#confirmChangeBtn, #codeGroup, #newPasswordGroup, #confirmPasswordGroup').removeClass('d-none');
            clearOTPFields();

            commonButton.setButtonLoading(btnId, false, "Send Mail");
        },
        function (err) {
            commonSwal.showError(err.responseJSON?.message);
            commonButton.setButtonLoading(btnId, false, "Send Mail");
        });

};
const confirmChangePassword = async () => {
    const email = $('#forgotEmail').val().trim();
    const code = getOTP();
    const newPassword = $('#newPassword').val();
    const confirmPassword = $('#confirmPassword').val();

    if (!email || code.length !== 6 || !newPassword || !confirmPassword) {
        return showAlert("Vui lòng nhập đầy đủ thông tin.", "warning");
    }

    const validateMsg = commonPassword.validatePassword(newPassword);
    if (validateMsg) return showAlert(validateMsg, "warning");

    if (newPassword !== confirmPassword) {
        return showAlert("Mật khẩu xác nhận không khớp.", "warning");
    }

    try {
        apiHelper.post(`/Account/ChangePassword`,
            { email, code, new_password: newPassword },
            () => {
                Swal.fire({
                    text: "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.",
                    icon: "success",
                    confirmButtonText: "Đăng nhập",
                    customClass: { confirmButton: "btn fw-bold btn-primary" }
                }).then(() => location.href = '/login');
            },
            (err) => {
                showAlert(err.responseJSON?.message || "Đã có lỗi xảy ra. Vui lòng thử lại.", "error");
            });
    } catch (err) {
        console.error(err);
        showAlert("Đã có lỗi xảy ra. Vui lòng thử lại.", "error");
    }
};
function showAlert(text, type = "warning") {
    Swal.fire({
        text: text,
        icon: type,
        confirmButtonText: "Ok, tôi hiểu!",
        customClass: { confirmButton: "btn fw-bold btn-primary" }
    });
}
