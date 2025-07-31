$(document).ready(function () {
    init();
});

const init = () => {
    // ✅ Lấy user_code từ URL
    const params = new URLSearchParams(window.location.search);
    const user_code = params.get('user_code');

    if (user_code) {
        console.log("User Code:", user_code);
        // Gọi hàm xử lý tiếp theo nếu cần
        apiHelper.get(`/User/Select-User`,
            { Emp_Code: user_code },
            function (res) {
                if (res && res.data) {
                    const user = res.data;
                    console.log("User Data:", user);
                    // Hiển thị thông tin người dùng
                    $("#userName").text(user.name || "Chưa có tên");
                    $("#userEmail").text(user.email || "Chưa có email");
                    $("#userPhone").text(user.phone || "Chưa có số điện thoại");
                    // Thêm các thông tin khác nếu cần
                } else
                    console.warn("Không tìm thấy dữ liệu người dùng");
            },
            function (err) {
                console.error("Lỗi lấy danh sách");
            });
    } else {
        console.warn("Không tìm thấy user_code trên URL");
    }
};
