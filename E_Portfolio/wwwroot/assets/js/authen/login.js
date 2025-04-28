var baseUrl = "";

$(document).ready(function () {
    // Xoá token trong localStorage
    localStorage.removeItem('e_atoken');
    localStorage.removeItem('e_rtoken');

    $("#togglePassword").click(function () {
        let passwordInput = $("#password");
        let icon = $(this).find("i");

        if (passwordInput.attr("type") === "password") {
            passwordInput.attr("type", "text");
            icon.removeClass("bi-eye").addClass("bi-eye-slash");
        } else {
            passwordInput.attr("type", "password");
            icon.removeClass("bi-eye-slash").addClass("bi-eye");
        }
    });

});

const login = async () => {
    let data = {
        email: $(`#email`).val().trim(),
        password: $(`#password`).val().trim(),
    };

    // Kiểm tra xem các trường có bị bỏ trống không
    if (!data.email) {
        Swal.fire({
            text: "Email không được để trống!",
            icon: "warning",
            buttonsStyling: !1,
            confirmButtonText: "Ok, tôi hiểu!",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        })
        return;
    }
    if (!data.password) {
        Swal.fire({
            text: "Mật khẩu không được để trống!",
            icon: "warning",
            buttonsStyling: !1,
            confirmButtonText: "Ok, tôi hiểu!",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        })
        return;
    }

    let loginBtn = document.getElementById("loginBtn");
    let errorMessage = document.getElementById("errorMessage");

    // Ẩn thông báo lỗi trước khi đăng nhập
    errorMessage.classList.add("d-none");

    // Đổi nút thành loading
    loginBtn.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loading...`;
    loginBtn.disabled = true;

    try {
        await apiHelper.post("login", data,
            async (res) => {
                var response = res.data;
                localStorage.setItem('e_atoken', response.access_token || null);
                localStorage.setItem('e_rtoken', response.refresh_token || null);
                localStorage.setItem('e_language', 'vn');
               
                window.location.href = "/";
                // Khôi phục lại nút Login
                loginBtn.innerHTML = "Login";
                loginBtn.disabled = false;
            },
            (err) => {
                errorMessage.textContent = err.responseJSON.message;
                errorMessage.classList.remove("d-none");
                // Khôi phục lại nút Login
                loginBtn.innerHTML = "Login";
                loginBtn.disabled = false;
            }
        );
    } catch (error) {
        console.error("Exception Error:", error);
        errorMessage.textContent = "Lỗi hệ thống. Vui lòng thử lại!";
        errorMessage.classList.remove("d-none");
        // Khôi phục lại nút Login
        loginBtn.innerHTML = "Login";
        loginBtn.disabled = false;
    }

};

