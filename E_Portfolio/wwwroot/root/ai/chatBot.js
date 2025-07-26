$(document).ready(function () {

});

const onImageSelected = (event) => {
    const preview = document.getElementById("analysisPreview");
    const resultImage = document.getElementById("analysisImage");
    const result = document.getElementById("analysisResult");

    const file = event.target.files[0];
    if (!file) return;

    // Hiển thị ảnh preview
    const reader = new FileReader();
    reader.onload = function () {
        const base64Data = reader.result;
        preview.src = base64Data;

        // Tách phần base64 (sau dấu phẩy)
        const base64 = base64Data.split(',')[1];

        // Gửi API nhận diện
        apiHelper.post(
            `/Image/detect-plate2`,
            { base64: base64 },
            function (res) {
                console.log("Kết quả nhận diện:", res);

                let objectHtml = '';
                if (res.objects.length > 0) {
                    objectHtml = res.objects.map((item, index) => {
                        const { label, text, color, box } = item;
                        return `
                            <div class="border rounded p-3 mb-3 shadow-sm">
                                <div class="fw-bold mb-2">🔹 Đối tượng ${index + 1}: <span class="text-primary">${label}</span></div>
                                <ul class="mb-0 ps-3 small">
                                    <li><strong>Text:</strong> ${text || ''}</li>
                                    <li><strong>Color:</strong> ${color || ''}</li>
                                    <li><strong>Box:</strong> x: ${box.x}, y: ${box.y}, width: ${box.width}, height: ${box.height}</li>
                                </ul>
                            </div>
                        `;
                    }).join('');
                }
                else {
                    objectHtml = `<div class="text-muted fst-italic">Không nhận diện được đối tượng nào.</div>`;
                }
                result.innerHTML = `
                    <div class="mb-3">
                        <div class="text-dark mt-2">
                            ${objectHtml || 'Không nhận diện được'}
                        </div>
                    </div>`;

                let html = `
                    <div class="mt-3">
                        <div class="border rounded shadow-sm overflow-hidden">
                            <img src="data:image/jpeg;base64,${res.annotated_image}" 
                            class="w-100 h-auto" alt="Ảnh kết quả"/>
                        </div>
                    </div>
                `;
                resultImage.innerHTML = html;
            },
            function (err) {
                console.error("Lỗi detect plate:", err);
                result.innerHTML = `
                    <div class="alert alert-danger">
                        ❌ Không thể nhận diện ảnh. Vui lòng thử lại.
                    </div>
                `;
            }
        );
    };

    reader.readAsDataURL(file);
};


function sendMessage() {
    const input = document.getElementById("chatInput");
    const chatBox = document.getElementById("chatBox");
    const message = input.value.trim();

    if (!message) return;

    // Tin nhắn người dùng
    chatBox.innerHTML += `
            <div class="d-flex flex-row-reverse align-items-start mt-2">
                <div class="badge text-white ms-2">
                    <i class="ki-duotone ki-user-edit fs-1 text-info">
                        <span class="path1"></span>
                        <span class="path2"></span>
                        <span class="path3"></span>
                    </i>
                </div>
                <div class="bg-white rounded p-2 shadow-sm">${message}</div>
            </div>
        `;
    input.value = "";
    chatBox.scrollTop = chatBox.scrollHeight;

    // Phản hồi giả lập
    setTimeout(() => {
        chatBox.innerHTML += `
                <div class="d-flex flex-row align-items-start mt-2">
                     <div class="badge text-white me-2">
                        <i class="ki-duotone ki-technology-3 fs-1 text-success">
                             <span class="path1"></span>
                             <span class="path2"></span>
                             <span class="path3"></span>
                             <span class="path4"></span>
                         </i>
                    </div>
                    <div class="bg-white rounded p-2 shadow-sm">
                        Cảm ơn bạn! Đây là phản hồi mẫu cho: <strong>${message}</strong>
                    </div>
                </div>
            `;
        chatBox.scrollTop = chatBox.scrollHeight;
    }, 500);
}