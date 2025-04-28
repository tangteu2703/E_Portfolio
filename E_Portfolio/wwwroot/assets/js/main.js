$(document).ready(async function () {
    checkAccess();
    const language_code = localStorage.getItem('e_language');

    commonFunction.languge.saveDictionaryToIndexedDB();

    commonLanguage.applyTranslations(language_code);

    loadGoogleTranslate();
});

// Khi tải trang, kiểm tra ngôn ngữ đã chọn
document.addEventListener("DOMContentLoaded", function () {
    // Lấy dữ liệu từ localStorage và parse
    const savedLang = localStorage.getItem('e_language') || 'vn';
    const langList = JSON.parse(localStorage.getItem('list_Language') || '[]');

    // Convert từ array
    const langMap = Object.fromEntries(langList.map(item => [item.code, item.name]));

    document.getElementById('selected-lang').innerText = langMap[savedLang] || 'Tiếng việt';
});
const checkAccess = async () => {
    const token = localStorage.getItem('e_atoken');
    const refresh_token = localStorage.getItem('e_rtoken');
    if (token === null) {
        window.location.href = '/login';
        return;
    }
    try {
        const decoded_token = commonFunction.extension.parseJwt(token);

        // Kiểm tra nếu token đã hết hạn
        const currentTime = Math.floor(Date.now() / 1000);
        if (decoded_token.exp && decoded_token.exp < currentTime) {
            console.warn('Token đã hết hạn, đang làm mới...');
            await commonFunction.extension.refreshToken(decoded_token.unique_name, 0, refresh_token);
        }

        // Cập nhật giao diện với thông tin từ token
        $('#lbl_username').text(decoded_token.unique_name !== undefined ? decoded_token.unique_name : 'unknown');
        if ($('#lbl_username').text() !== 'Administrator') {
            $("#proTag").addClass("d-none");
        }
        $('#lbl_email').text(decoded_token.email ? decoded_token.email : 'unknown');
        const avatarUrl = decoded_token.avatar ? decoded_token.avatar : 'new_asset_temp/media/avatars/blank.png';
        $("#imgAvatar2").attr("src", avatarUrl);
        $("#imgAvatar6").attr("src", avatarUrl);

    } catch (error) {
        console.error('Token không hợp lệ:', error);
    }
};

// Gọi script của Google Translate
function loadGoogleTranslate() {
    const script = document.createElement("script");
    script.type = "text/javascript";
    script.src = "//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit";
    document.body.appendChild(script);
}

// Hàm init Google Translate
function googleTranslateElementInit() {
    new google.translate.TranslateElement({
        pageLanguage: 'vi',
        includedLanguages: 'vi,en,fr,ja,ko,zh,es,de,th,ru,ar,it,pt,hi',
        autoDisplay: false
    }, 'google_translate_element');
}
const hideSkipTranslateElements = () => {
    // Tìm tất cả các phần tử có class 'skiptranslate'
    const skiptranslateElements = document.querySelectorAll('.skiptranslate');

    // Gán 'display: none' cho tất cả các phần tử đó
    skiptranslateElements.forEach(element => {
        element.style.display = 'none';
    });
};

// Hàm dịch khi chọn ngôn ngữ
const changeLanguage = async (language_code, langText) => {
    if (language_code !== 'vn') {
        const select = document.querySelector("select.goog-te-combo");
        if (select) {
            select.value = language_code;
            select.dispatchEvent(new Event("change"));
        }
    }

    // Lưu vào localStorage
    localStorage.setItem('e_language', language_code);

    // Cập nhật giao diện
    document.getElementById('selected-lang').innerText = langText;

    commonLanguage.applyTranslations(language_code);

    //hideSkipTranslateElements();
}

(function () {
    "use strict";

    /**
     * Header toggle
     */
    const headerToggleBtn = document.querySelector('.header-toggle');

    function headerToggle() {
        document.querySelector('#header').classList.toggle('header-show');
        headerToggleBtn.classList.toggle('bi-list');
        headerToggleBtn.classList.toggle('bi-x');
    }
    headerToggleBtn.addEventListener('click', headerToggle);

    /**
     * Hide mobile nav on same-page/hash links
     */
    document.querySelectorAll('#navmenu a').forEach(navmenu => {
        navmenu.addEventListener('click', () => {
            if (document.querySelector('.header-show')) {
                headerToggle();
            }
        });

    });

    /**
     * Toggle mobile nav dropdowns
     */
    document.querySelectorAll('.navmenu .toggle-dropdown').forEach(navmenu => {
        navmenu.addEventListener('click', function (e) {
            e.preventDefault();
            this.parentNode.classList.toggle('active');
            this.parentNode.nextElementSibling.classList.toggle('dropdown-active');
            e.stopImmediatePropagation();
        });
    });

    /**
     * Preloader
     */
    const preloader = document.querySelector('#preloader');
    if (preloader) {
        window.addEventListener('load', () => {
            preloader.remove();
        });
    }

    /**
     * Scroll top button
     */
    let scrollTop = document.querySelector('.scroll-top');

    function toggleScrollTop() {
        if (scrollTop) {
            window.scrollY > 100 ? scrollTop.classList.add('active') : scrollTop.classList.remove('active');
        }
    }
    scrollTop.addEventListener('click', (e) => {
        e.preventDefault();
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });

    window.addEventListener('load', toggleScrollTop);
    document.addEventListener('scroll', toggleScrollTop);

    /**
     * Animation on scroll function and init
     */
    function aosInit() {
        AOS.init({
            duration: 600,
            easing: 'ease-in-out',
            once: true,
            mirror: false
        });
    }
    window.addEventListener('load', aosInit);

    /**
     * Init typed.js
     */
    const selectTyped = document.querySelector('.typed');
    if (selectTyped) {
        let typed_strings = selectTyped.getAttribute('data-typed-items');
        typed_strings = typed_strings.split(',');
        new Typed('.typed', {
            strings: typed_strings,
            loop: true,
            typeSpeed: 100,
            backSpeed: 50,
            backDelay: 2000
        });
    }

    /**
     * Initiate Pure Counter
     */
    new PureCounter();

    /**
     * Animate the skills items on reveal
     */
    let skillsAnimation = document.querySelectorAll('.skills-animation');
    skillsAnimation.forEach((item) => {
        new Waypoint({
            element: item,
            offset: '80%',
            handler: function (direction) {
                let progress = item.querySelectorAll('.progress .progress-bar');
                progress.forEach(el => {
                    el.style.width = el.getAttribute('aria-valuenow') + '%';
                });
            }
        });
    });

    /**
     * Initiate glightbox
     */
    const glightbox = GLightbox({
        selector: '.glightbox'
    });

    /**
     * Init isotope layout and filters
     */
    document.querySelectorAll('.isotope-layout').forEach(function (isotopeItem) {
        let layout = isotopeItem.getAttribute('data-layout') ?? 'masonry';
        let filter = isotopeItem.getAttribute('data-default-filter') ?? '*';
        let sort = isotopeItem.getAttribute('data-sort') ?? 'original-order';

        let initIsotope;
        imagesLoaded(isotopeItem.querySelector('.isotope-container'), function () {
            initIsotope = new Isotope(isotopeItem.querySelector('.isotope-container'), {
                itemSelector: '.isotope-item',
                layoutMode: layout,
                filter: filter,
                sortBy: sort
            });
        });

        isotopeItem.querySelectorAll('.isotope-filters li').forEach(function (filters) {
            filters.addEventListener('click', function () {
                isotopeItem.querySelector('.isotope-filters .filter-active').classList.remove('filter-active');
                this.classList.add('filter-active');
                initIsotope.arrange({
                    filter: this.getAttribute('data-filter')
                });
                if (typeof aosInit === 'function') {
                    aosInit();
                }
            }, false);
        });

    });

    /**
     * Init swiper sliders
     */
    function initSwiper() {
        document.querySelectorAll(".init-swiper").forEach(function (swiperElement) {
            let config = JSON.parse(
                swiperElement.querySelector(".swiper-config").innerHTML.trim()
            );

            if (swiperElement.classList.contains("swiper-tab")) {
                initSwiperWithCustomPagination(swiperElement, config);
            } else {
                new Swiper(swiperElement, config);
            }
        });
    }

    window.addEventListener("load", initSwiper);

    /**
     * Correct scrolling position upon page load for URLs containing hash links.
     */
    window.addEventListener('load', function (e) {
        if (window.location.hash) {
            if (document.querySelector(window.location.hash)) {
                setTimeout(() => {
                    let section = document.querySelector(window.location.hash);
                    let scrollMarginTop = getComputedStyle(section).scrollMarginTop;
                    window.scrollTo({
                        top: section.offsetTop - parseInt(scrollMarginTop),
                        behavior: 'smooth'
                    });
                }, 100);
            }
        }
    });

    /**
     * Navmenu Scrollspy
     */
    let navmenulinks = document.querySelectorAll('.navmenu a');

    function navmenuScrollspy() {
        navmenulinks.forEach(navmenulink => {
            if (!navmenulink.hash) return;
            let section = document.querySelector(navmenulink.hash);
            if (!section) return;
            let position = window.scrollY + 200;
            if (position >= section.offsetTop && position <= (section.offsetTop + section.offsetHeight)) {
                document.querySelectorAll('.navmenu a.active').forEach(link => link.classList.remove('active'));
                navmenulink.classList.add('active');
            } else {
                navmenulink.classList.remove('active');
            }
        })
    }
    window.addEventListener('load', navmenuScrollspy);
    document.addEventListener('scroll', navmenuScrollspy);

})();