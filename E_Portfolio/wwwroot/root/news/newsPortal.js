// API Configuration
const API_BASE_URL = '/api/News';
let currentPage = 1;
const pageSize = 10;
let selectedImages = [];
let selectedFeeling = '';
let newsList = [];
$(document).ready(function () {
    init();
    initCreatePostForm();
});

const init = () => {
    loadNewsList();
};

// Load news list from API
async function loadNewsList(page = 1) {
    try {
        showLoadingSpinner();

        const response = await fetch(`${API_BASE_URL}/GetNewsList?page_index=${page}&page_size=${pageSize}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Failed to load news');
        }

        const data = await response.json();
        renderNews(data);
        currentPage = page;

    } catch (error) {
        console.error('Error loading news:', error);
        showError('Không thể tải bài viết. Đang sử dụng dữ liệu demo.');
        // Fallback to demo data
        loadDemoData();
    }
}

// Fallback demo data
function loadDemoData() {
    var newsList = [
        {
            news_id: 1,
            user_code: "NV001",
            title: "Tin tức 1",
            contents: "Lưu giữ những bức ảnh đẹp của tôi",
            enter_time: "2024-10-01",
            status: "vui vẻ",
            tagging: ["NV001", "NV002"], // gắn thẻ ai trong bài viết
            images: ["/assetsCMS/media/stock/600x600/img-55.jpg", "/assetsCMS/media/stock/600x600/img-60.jpg", "/assetsCMS/media/stock/600x600/img-56.jpg", "/assetsCMS/media/stock/600x600/img-63.jpg", "/assetsCMS/media/stock/600x600/img-58.jpg"], // hình ảnh trong bài viết
            likes: [   // cảm xúc của bài viết
                {
                    type_id: 1,  //like
                    count: 101,
                },
                {
                    type_id: 2, //love
                    count: 20,
                },
                {
                    type_id: 3, //haha
                    count: 11
                },
            ],
            conments: [
                {
                    comment_id: 1,
                    parent_id: 0,
                    user_code: "NV001",
                    content: "ảnh thật đẹp",
                    enter_time: "2024-10-01 10:00",
                    images: ["img-11.jpg"], // hinh ảnh trong bình luận
                    likes: [  // cảm xúc của bình luận
                        {
                            type_id: 1,  //like
                            count: 1,
                        },
                        {
                            type_id: 2, //love
                            count: 1,
                        },
                        {
                            type_id: 3, //haha
                            count: 1
                        },
                    ],
                },
                {
                    comment_id: 2,
                    parent_id: 0,
                    user_code: "NV002",
                    content: "Phải làm sao... Phải chịu",
                    images: ["img-12.jpg"], // hinh ảnh trong bình luận
                    enter_time: "2024-10-01 10:00",
                    likes: [  // cảm xúc của bình luận
                        {
                            type_id: 1,  //like
                            count: 1,
                        },
                        {
                            type_id: 2, //love
                            count: 1,
                        },
                        {
                            type_id: 3, //haha
                            count: 1
                        },
                    ],
                },
            ]
        },
        {
            news_id: 2,
            user_code: "NV002",
            title: "Tin tức 2",
            contents: "Nhiều hoạt động thú vị tại phố đi bộ Nguyễn Huệ dịp tuần lễ ‘Khỏe để xây dựng và bảo vệ Tổ quốc’",
            enter_time: "2024-10-21",
            status: "thú vị",
            tagging: [], // gắn thẻ ai trong bài viết
            images: [], // hình ảnh trong bài viết
            likes: [   // cảm xúc của bài viết
                {
                    type_id: 1,  //like
                    count: 101,
                },
                {
                    type_id: 2, //love
                    count: 20,
                },
                {
                    type_id: 3, //haha
                    count: 11
                },
            ],
            conments: [
                {
                    comment_id: 11,
                    parent_id: 0,
                    user_code: "NV001",
                    content: "Cuối tuần này nhé bro.",
                    enter_time: "2024-10-01 10:00",
                    images: [], // hình ảnh trong bình luận
                    likes: [  // cảm xúc của bình luận
                        {
                            type_id: 1,  //like
                            count: 1,
                        },
                        {
                            type_id: 2, //love
                            count: 1,
                        },
                        {
                            type_id: 3, //haha
                            count: 1
                        },
                    ],
                },
            ],
            shared: [
                {
                    user_code: "NV006",
                    enter_time: "2024-10-22 10:00",
                },
                {
                    user_code: "NV008",
                    enter_time: "2024-10-23 10:00",
                },
            ]
        },
    ];
    renderNews(newsList);
}

// Helper functions
function getAuthToken() {
    // Get token from localStorage or cookie
    return localStorage.getItem('authToken') || '';
}

function showLoadingSpinner() {
    $('#kt_social_feeds_posts').html(`
        <div class="text-center py-10">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Đang tải...</span>
            </div>
        </div>
    `);
}

function showError(message) {
    toastr.error(message);
}

function showSuccess(message) {
    toastr.success(message);
}

// Initialize Create Post Form
function initCreatePostForm() {
    // Feeling selector
    $('.feeling-option').on('click', function (e) {
        e.preventDefault();
        const feeling = $(this).data('feeling');
        const icon = $(this).data('icon');

        selectedFeeling = feeling;

        if (feeling) {
            $('#selected_feeling_text').text(`${icon} ${feeling}`);
        } else {
            $('#selected_feeling_text').text('Cảm xúc');
        }
        $('#post_status').val(feeling);
    });

    // Image upload button
    $('#btn_add_images').on('click', function () {
        $('#post_images_input').click();
    });

    // Handle image selection
    $('#post_images_input').on('change', function (e) {
        const files = Array.from(e.target.files);

        if (files.length > 0) {
            selectedImages = files;
            displayImagePreviews(files);
            $('#image_preview_container').removeClass('d-none');
        }
    });

    // Close image preview
    $('#close_image_preview').on('click', function () {
        $('#image_preview_container').addClass('d-none');
        $('#post_images_input').val('');
        selectedImages = [];
    });

    // Submit form
    $('#kt_modal_create_post_form').on('submit', async function (e) {
        e.preventDefault();
        await createPost();
    });

    // Load more posts
    $('#kt_social_feeds_more_posts_btn').on('click', function (e) {
        e.preventDefault();
        loadNewsList(currentPage + 1);
    });
}

// Display image previews
function displayImagePreviews(files) {
    const previewGrid = $('#image_preview_grid');
    previewGrid.empty();

    files.forEach((file, index) => {
        const reader = new FileReader();
        reader.onload = function (e) {
            const colClass = files.length === 1 ? 'col-12' : 'col-6';
            const html = `
                <div class="${colClass}">
                    <div class="card-rounded position-relative" style="height: 150px;">
                        <img src="${e.target.result}" class="w-100 h-100 object-fit-cover card-rounded" />
                    </div>
                </div>
            `;
            previewGrid.append(html);
        };
        reader.readAsDataURL(file);
    });
}

// Create new post
async function createPost() {
    const submitBtn = $('#kt_modal_create_post_submit');
    const content = $('#post_content').val();
    const status = $('#post_status').val();

    if (!content && selectedImages.length === 0) {
        showError('Vui lòng nhập nội dung hoặc chọn hình ảnh');
        return;
    }

    try {
        // Show loading
        submitBtn.attr('data-kt-indicator', 'on');
        submitBtn.prop('disabled', true);

        // Create FormData
        const formData = new FormData();
        formData.append('contents', content);
        formData.append('status', status);
        formData.append('privacy_level', 1); // Public

        // Append images
        selectedImages.forEach((image, index) => {
            formData.append('images', image);
        });

        // Send request
        const response = await fetch(`${API_BASE_URL}/Create`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            body: formData
        });

        if (!response.ok) {
            throw new Error('Failed to create post');
        }

        const result = await response.json();

        showSuccess('Đăng bài thành công!');

        // Reset form
        $('#kt_modal_create_post_form')[0].reset();
        $('#image_preview_container').addClass('d-none');
        $('#post_images_input').val('');
        selectedImages = [];
        selectedFeeling = '';
        $('#selected_feeling_text').text('Cảm xúc');

        // Close modal
        $('#kt_modal_create_post').modal('hide');

        // Reload news list
        loadNewsList(1);

    } catch (error) {
        console.error('Error creating post:', error);
        showError('Không thể đăng bài. Vui lòng thử lại.');
    } finally {
        submitBtn.removeAttr('data-kt-indicator');
        submitBtn.prop('disabled', false);
    }
}

// Add comment to post
async function addComment(newsId, content) {
    try {
        const formData = new FormData();
        formData.append('news_id', newsId);
        formData.append('content', content);

        const response = await fetch(`${API_BASE_URL}/Comment`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`
            },
            body: formData
        });

        if (!response.ok) {
            throw new Error('Failed to add comment');
        }

        showSuccess('Đã thêm bình luận');
        // Reload comments or update UI

    } catch (error) {
        console.error('Error adding comment:', error);
        showError('Không thể thêm bình luận');
    }
}

// Toggle reaction
async function toggleReaction(newsId, reactionType) {
    try {
        const response = await fetch(`${API_BASE_URL}/Reaction`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${getAuthToken()}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                news_id: newsId,
                reaction_type: reactionType
            })
        });

        if (!response.ok) {
            throw new Error('Failed to toggle reaction');
        }

        // Update UI to reflect reaction change

    } catch (error) {
        console.error('Error toggling reaction:', error);
    }
}

// Helper: Format time ago
const formatTimeAgo = (dateString) => {
    const date = new Date(dateString);
    const now = new Date();
    const diff = Math.floor((now - date) / 1000); // seconds

    if (diff < 60) return 'Vừa xong';
    if (diff < 3600) return `${Math.floor(diff / 60)} phút trước`;
    if (diff < 86400) return `${Math.floor(diff / 3600)} giờ trước`;
    if (diff < 172800) return 'Hôm qua lúc ' + date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
    if (diff < 604800) return `${Math.floor(diff / 86400)} ngày trước`;
    return date.toLocaleDateString('vi-VN');
};

// Helper: Get total likes count
const getTotalLikes = (likes) => {
    return likes.reduce((sum, like) => sum + like.count, 0);
};

// Helper: Get total shares count
const getTotalShares = (shared) => {
    return shared ? shared.length : 0;
};

// Helper: Get user info (mock data - replace with real API call)
const getUserInfo = (userCode) => {
    const users = {
        'NV001': { name: 'Nguyễn Văn An', avatar: '/assetsCMS/media/avatars/300-1.jpg' },
        'NV002': { name: 'Trần Thị Bình', avatar: '/assetsCMS/media/avatars/300-2.jpg' },
        'NV003': { name: 'Lê Văn Cường', avatar: '/assetsCMS/media/avatars/300-3.jpg' },
        'NV004': { name: 'Phạm Thị Dung', avatar: '/assetsCMS/media/avatars/300-4.jpg' },
        'NV005': { name: 'Hoàng Văn Em', avatar: '/assetsCMS/media/avatars/300-5.jpg' },
        'NV006': { name: 'Đỗ Thị Phương', avatar: '/assetsCMS/media/avatars/300-6.jpg' },
        'NV007': { name: 'Bùi Văn Giang', avatar: '/assetsCMS/media/avatars/300-7.jpg' },
        'NV008': { name: 'Vũ Thị Hoa', avatar: '/assetsCMS/media/avatars/300-8.jpg' },
    };
    return users[userCode] || { name: 'Unknown User', avatar: '/assetsCMS/media/avatars/blank.png' };
};

// Helper: Render images based on count (Facebook style)
const renderImages = (images) => {
    if (!images || images.length === 0) return '';

    const imgCount = images.length;
    let html = '';
    if (imgCount === 1) {
        // 1 ảnh: Full width
        html = `
            <div class="row g-2 mb-4">
                <div class="col-12">
                    <a class="d-block card-rounded overlay h-300px h-md-400px" data-fslightbox="lightbox-projects">
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                            <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        </div>
                    </a>
                </div>
            </div>`;
    } else if (imgCount === 2) {
        // 2 ảnh: Chia đôi
        html = `
            <div class="row g-2 mb-4">
                <div class="col-6">
                    <a class="d-block card-rounded overlay h-250px h-md-300px" data-fslightbox="lightbox-projects">
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[0]}')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                            <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        </div>
                    </a>
                </div>
                <div class="col-6">
                    <a class="d-block card-rounded overlay h-250px h-md-300px" data-fslightbox="lightbox-projects">
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[1]}')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                            <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        </div>
                    </a>
                </div>
            </div>`;
    } else if (imgCount === 3) {
        // 3 ảnh: 1 to bên trái, 2 nhỏ bên phải
        html = `
            <div class="row g-2 mb-4">
                <div class="col-6">
                    <a class="d-block card-rounded overlay h-250px h-md-350px" data-fslightbox="lightbox-projects" >
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[0]}')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                            <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        </div>
                    </a>
                </div>
                <div class="col-6">
                    <div class="row g-2">
                        <div class="col-12">
                            <a class="d-block card-rounded overlay h-120px h-md-170px" data-fslightbox="lightbox-projects">
                                <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[1]}')"></div>
                                <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                                    <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                                </div>
                            </a>
                        </div>
                        <div class="col-12">
                            <a class="d-block card-rounded overlay h-120px h-md-170px" data-fslightbox="lightbox-projects">
                                <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[2]}')"></div>
                                <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                                    <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                                </div>
                            </a>
                        </div>
                    </div>
                </div>
            </div>`;
    } else {
        // 4+ ảnh: Hiển thị 4 ảnh đầu, ảnh cuối có overlay "+X" để xem thêm
        const remainingCount = imgCount - 4;
        html = `
            <div class="row g-2 mb-4">
                <div class="col-6">
                    <a class="d-block card-rounded overlay h-150px h-md-200px" data-fslightbox="lightbox-projects">
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[0]}')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                            <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        </div>
                    </a>
                </div>
                <div class="col-6">
                    <a class="d-block card-rounded overlay h-150px h-md-200px" data-fslightbox="lightbox-projects">
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[1]}')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                            <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        </div>
                    </a>
                </div>
                <div class="col-6">
                    <a class="d-block card-rounded overlay h-150px h-md-200px" data-fslightbox="lightbox-projects">
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[2]}')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-25">
                            <i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        </div>
                    </a>
                </div>
                <div class="col-6">
                    <a class="d-block card-rounded overlay h-150px h-md-200px position-relative" data-fslightbox="lightbox-projects">
                        <div class="overlay-wrapper bgi-no-repeat bgi-position-center bgi-size-cover card-rounded h-100" style="background-image:url('${images[3]}')"></div>
                        <div class="overlay-layer card-rounded bg-dark bg-opacity-${remainingCount > 0 ? '75' : '25'} d-flex align-items-center justify-content-center">
                            ${remainingCount > 0 ? `<span class="fs-1 fw-bold text-white">+${remainingCount}</span>` : `<i class="ki-duotone ki-eye fs-3x text-white"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>`}
                        </div>
                    </a>
                </div>
            </div>`;
    }

    return html;
};

// Helper: Render top 2 comments with most interactions
const renderTopComments = (comments, newsId) => {
    if (!comments || comments.length === 0) return '';

    // Sort comments by total likes (descending)
    const sortedComments = [...comments].sort((a, b) => {
        const aLikes = getTotalLikes(a.likes || []);
        const bLikes = getTotalLikes(b.likes || []);
        return bLikes - aLikes;
    });

    // Get top 2 comments
    const topComments = sortedComments.slice(0, 2);

    let html = `<div class="collapse show" id="kt_social_feeds_comments_${newsId}">`;

    topComments.forEach(comment => {
        const userInfo = getUserInfo(comment.user_code);
        const timeAgo = formatTimeAgo(comment.enter_time);
        const totalLikes = getTotalLikes(comment.likes || []);

        html += `
            <div class="d-flex pt-6">
                <div class="symbol symbol-45px me-5">
                    <img src="${userInfo.avatar}" alt="${userInfo.name}">
                </div>
                <div class="d-flex flex-column flex-row-fluid">
                    <div class="d-flex align-items-center flex-wrap mb-1">
                        <a href="#" class="text-gray-800 text-hover-primary fw-bold me-2">${userInfo.name}</a>
                        <span class="text-gray-500 fw-semibold fs-7 me-2">${timeAgo}</span>
                        ${totalLikes > 0 ? `<span class="badge badge-light-primary fs-8">${totalLikes} <i class="ki-duotone ki-heart fs-7"><span class="path1"></span><span class="path2"></span></i></span>` : ''}
                    </div>
                    <div class="bg-light-gray rounded p-3 mb-2">
                        <span class="text-gray-800 fs-7 fw-normal">${comment.content}</span>
                    </div>
                    ${comment.images && comment.images.length > 0 ? `
                        <div class="d-flex gap-2 mb-2">
                            ${comment.images.map(img => `
                                <img src="/assetsCMS/media/stock/600x600/${img}" class="rounded" style="max-width: 150px; max-height: 150px; object-fit: cover;" alt="Comment image">
                            `).join('')}
                        </div>
                    ` : ''}
                    <div class="d-flex align-items-center gap-3">
                        <a href="#" class="text-gray-500 text-hover-primary fw-semibold fs-7">Thích</a>
                        <a href="#" class="text-gray-500 text-hover-primary fw-semibold fs-7">Phản hồi</a>
                    </div>
                </div>
            </div>`;
    });

    html += '</div>';
    return html;
};

const renderNews = (newsList) => {
    const newsContainer = $('#kt_social_feeds_posts');
    newsContainer.empty();
    newsList = newsList;
    newsList.forEach(news => {
        const userInfo = getUserInfo(news.user_code);
        const timeAgo = formatTimeAgo(news.enter_time);
        const totalLikes = getTotalLikes(news.likes || []);
        const totalComments = news.conments ? news.conments.length : 0;
        const totalShares = getTotalShares(news.shared);
        const imagesHtml = renderImages(news.images);
        const commentsHtml = renderTopComments(news.conments, news.news_id);

        const newsCard = `
            <div class="card card-flush mb-10">
                <!-- Header -->
                <div class="card-header pt-9 pb-4">
                    <div class="d-flex align-items-center">
                        <div class="symbol symbol-50px me-3">
                            <img src="${userInfo.avatar}" class="rounded-circle" alt="${userInfo.name}">
                        </div>
                        <div class="flex-grow-1">
                            <a href="#" class="text-gray-800 text-hover-primary fs-5 fw-bold d-block">${userInfo.name} ${news.status ? ` - đang cảm thấy <span class="badge badge-light-info fs-5">${news.status}</span>` : ''}.</a>
                            <div class="d-flex align-items-center gap-2">
                                <span class="text-gray-500 fw-semibold fs-7">${timeAgo}</span>
                            </div>
                        </div>
                     </div>
                     <div class="card-toolbar">
                     <div class="dropdown">
                            <button class="btn btn-sm btn-icon btn-color-gray-500 btn-active-light" type="button" data-bs-toggle="dropdown">
                                <i class="ki-duotone ki-dots-vertical fs-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                            </button>
                            <ul class="dropdown-menu dropdown-menu-end">
                                <li><a class="dropdown-item" href="#">Chỉnh sửa</a></li>
                                <li><a class="dropdown-item" href="#">Xóa bài viết</a></li>
                                <li><a class="dropdown-item" href="#">Báo cáo</a></li>
                            </ul>
                      </div>
                    </div>
                </div>

                <!-- Body -->
                <div class="card-body pt-2 pb-4 view-post-detail" data-news-id="${news.news_id}" style="cursor: pointer;">
                    ${news.contents ? `<div class="fs-6 fw-normal text-gray-800 mb-4">${news.contents}</div>` : ''}
                    ${imagesHtml}
                </div>

                <!-- Footer -->
                <div class="card-footer pt-0 pb-4">
                    <!-- Stats -->
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <div class="d-flex align-items-center gap-2">
                            ${totalLikes > 0 ? `
                                <div class="d-flex align-items-center">
                                    <span class="text-gray-500 fs-7">${totalLikes} lượt thích</span>
                                </div>
                            ` : ''}
                        </div>
                        <div class="d-flex gap-3">
                            ${totalComments > 0 ? `<span class="text-gray-500 fs-7">${totalComments} bình luận</span>` : ''}
                            ${totalShares > 0 ? `<span class="text-gray-500 fs-7">${totalShares} chia sẻ</span>` : ''}
                        </div>
                    </div>

                    <div class="separator separator-solid mb-3"></div>

                    <!-- Actions -->
                    <div class="d-flex justify-content-around mb-4">
                        <button class="btn btn-sm btn-light btn-color-gray-700 btn-active-light-primary flex-fill me-2">
                            <i class="ki-duotone ki-heart fs-2 me-1"><span class="path1"></span><span class="path2"></span></i>
                            Thích
                        </button>
                        <button class="btn btn-sm btn-light btn-color-gray-700 btn-active-light-primary flex-fill me-2" data-bs-toggle="collapse" data-bs-target="#kt_social_feeds_comments_${news.news_id}">
                            <i class="ki-duotone ki-message-text-2 fs-2 me-1"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                            Bình luận
                        </button>
                        <button class="btn btn-sm btn-light btn-color-gray-700 btn-active-light-primary flex-fill">
                            <i class="ki-duotone ki-share fs-2 me-1"><span class="path1"></span><span class="path2"></span></i>
                            Chia sẻ
                        </button>
                    </div>

                    <div class="separator separator-solid mb-3"></div>

                    <!-- Comments -->
                    ${commentsHtml}

                    ${totalComments > 2 ? `
                        <div class="text-center mt-3">
                            <a href="#" class="text-primary fw-semibold fs-7">Xem thêm ${totalComments - 2} bình luận khác</a>
                        </div>
                    ` : ''}

                    <!-- Comment Form -->
                    <div class="d-flex align-items-center mt-4">
                        <div class="symbol symbol-40px me-3">
                            <img class="user_avatar rounded-circle" alt="Avatar" src="" />
                        </div>
                        <div class="position-relative flex-grow-1">
                            <input type="text" class="form-control form-control-solid rounded-pill ps-4 pe-12" placeholder="Viết bình luận..." />
                            <div class="position-absolute top-50 end-0 translate-middle-y me-3">
                                <button class="btn btn-icon btn-sm btn-light-primary rounded-circle p-0 me-1" title="Đính kèm ảnh">
                                    <i class="ki-duotone ki-picture fs-2"><span class="path1"></span><span class="path2"></span></i>
                                </button>
                                <button class="btn btn-icon btn-sm btn-light-primary rounded-circle p-0" title="Gửi">
                                    <i class="ki-duotone ki-send fs-2"><span class="path1"></span><span class="path2"></span></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

        newsContainer.append(newsCard);
    });

    // Add click event for viewing post detail
    $('.view-post-detail').on('click', function (e) {
        e.preventDefault();
        const newsId = $(this).data('news-id');
        viewPostDetail(newsId);
    });
}

// View post detail in modal
function viewPostDetail(newsId) {
    // Find the news in the list
    const news = newsList.find(n => n.news_id === newsId);

    if (!news) {
        console.error('News not found:', newsId);
        return;
    }

    const userInfo = getUserInfo(news.user_code);
    const timeAgo = formatTimeAgo(news.enter_time);
    const totalLikes = getTotalLikes(news.likes);
    const totalShares = getTotalShares(news.shared);

    // Build detail HTML
    let detailHTML = `
        <div class="card card-flush">
            <!-- Post Header -->
            <div class="card-header min-h-50px px-7">
                <div class="d-flex align-items-center flex-grow-1">
                    <div class="symbol symbol-45px me-3">
                        <img src="${userInfo.avatar}" alt="${userInfo.name}" />
                    </div>
                    <div class="flex-grow-1">
                        <a href="#" class="text-gray-800 text-hover-primary fs-5 fw-bold d-block">${userInfo.name} ${news.status ? ` - đang cảm thấy <span class="badge badge-light-info fs-6">${news.status}</span>` : ''}.</a>
                        <div class="d-flex align-items-center gap-2">
                            <span class="text-gray-500 fw-semibold fs-7">${timeAgo}</span>
                            <i class="ki-duotone ki-global fs-7 text-gray-500">
                                <span class="path1"></span>
                                <span class="path2"></span>
                            </i>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Post Body -->
            <div class="card-body px-7 pt-5">
                ${news.contents ? `<div class="mb-5 fs-4 text-gray-800" style="white-space: pre-wrap;">${news.contents}</div>` : ''}
                ${renderImages(news.images || [])}
            </div>
            
            <!-- Post Stats -->
            <div class="card-footer px-7 pt-3 pb-0">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div class="d-flex align-items-center">
                        ${totalLikes > 0 ? `
                            <div class="d-flex me-2">
                                <span class="badge badge-circle badge-primary me-n2" style="width: 20px; height: 20px;">
                                    <i class="ki-duotone ki-heart fs-7 text-white"><span class="path1"></span><span class="path2"></span></i>
                                </span>
                            </div>
                            <span class="text-gray-700 fw-semibold">${totalLikes}</span>
                        ` : ''}
                    </div>
                    <div class="text-gray-700 fw-semibold">
                        ${news.conments && news.conments.length > 0 ? `<span class="me-3">${news.conments.length} bình luận</span>` : ''}
                        ${totalShares > 0 ? `<span>${totalShares} chia sẻ</span>` : ''}
                    </div>
                </div>
                
                <div class="separator mb-3"></div>
                
                <!-- Action Buttons -->
                <div class="d-flex justify-content-around mb-3">
                    <button class="btn btn-sm btn-light btn-color-gray-700 btn-active-light-primary flex-fill me-2">
                        <i class="ki-duotone ki-heart fs-2 me-1"><span class="path1"></span><span class="path2"></span></i>
                        Thích
                    </button>
                    <button class="btn btn-sm btn-light btn-color-gray-700 btn-active-light-primary flex-fill me-2">
                        <i class="ki-duotone ki-message-text-2 fs-2 me-1"><span class="path1"></span><span class="path2"></span><span class="path3"></span></i>
                        Bình luận
                    </button>
                    <button class="btn btn-sm btn-light btn-color-gray-700 btn-active-light-primary flex-fill">
                        <i class="ki-duotone ki-share fs-2 me-1"><span class="path1"></span><span class="path2"></span></i>
                        Chia sẻ
                    </button>
                </div>
                
                <div class="separator mb-4"></div>
                
                <!-- All Comments -->
                <div class="mb-5">
                    ${renderAllComments(news.conments || [], news.news_id)}
                </div>
                
                <!-- Comment Form -->
                <div class="d-flex align-items-start mb-3">
                    <div class="symbol symbol-35px me-3">
                        <img class="user_avatar" alt="Avatar" src="" />
                    </div>
                    <div class="flex-grow-1">
                        <div class="position-relative">
                            <textarea class="form-control form-control-flush border border-gray-300 rounded ps-3" rows="1" 
                                placeholder="Viết bình luận..." style="resize: none;"></textarea>
                            <div class="position-absolute end-0 bottom-0 me-3 mb-2">
                                <button class="btn btn-sm btn-icon btn-active-color-primary">
                                    <i class="ki-duotone ki-message-add fs-2">
                                        <span class="path1"></span>
                                        <span class="path2"></span>
                                        <span class="path3"></span>
                                    </i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Show modal
    $('#post_detail_content').html(detailHTML);
    $('#kt_modal_view_post').modal('show');

    // Re-init fslightbox for images in modal
    if (typeof refreshFsLightbox !== 'undefined') {
        refreshFsLightbox();
    }
}

// Render all comments for detail view
function renderAllComments(comments, newsId) {
    if (!comments || comments.length === 0) {
        return '<div class="text-center text-muted py-5">Chưa có bình luận nào</div>';
    }

    let html = '';

    comments.forEach(comment => {
        const userInfo = getUserInfo(comment.user_code);
        const timeAgo = formatTimeAgo(comment.enter_time);
        const totalLikes = getTotalLikes(comment.likes);

        html += `
            <div class="d-flex align-items-start mb-5">
                <div class="symbol symbol-35px me-3">
                    <img src="${userInfo.avatar}" alt="${userInfo.name}" />
                </div>
                <div class="flex-grow-1">
                    <div class="bg-light-primary rounded p-4 mb-2">
                        <a href="#" class="text-gray-900 fw-bold fs-6">${userInfo.name}</a>
                        <div class="text-gray-800 fs-6 mt-1">${comment.content}</div>
                    </div>
                    <div class="d-flex align-items-center gap-4 mb-2">
                        <a href="#" class="text-gray-600 text-hover-primary fw-semibold fs-7">${timeAgo}</a>
                        ${totalLikes > 0 ? `
                            <span class="text-gray-600 fw-semibold fs-7">
                                <i class="ki-duotone ki-heart fs-7 text-danger"><span class="path1"></span><span class="path2"></span></i>
                                ${totalLikes}
                            </span>
                        ` : ''}
                        <a href="#" class="text-gray-600 text-hover-primary fw-semibold fs-7">Thích</a>
                        <a href="#" class="text-gray-600 text-hover-primary fw-semibold fs-7">Phản hồi</a>
                    </div>
                </div>
            </div>
        `;
    });

    return html;
}


