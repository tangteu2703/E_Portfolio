$(document).ready(async () => {
    await loadAndRenderMenu();
});

// 🔹 LOAD MENU FROM INDEXEDDB
const loadAndRenderMenu = async () => {
    try {
        const permissions = await commonIndexDB.getAllFromIndexedDB('AuthorizeDB', 'e_permissions');
        if (!permissions?.length) {
            console.warn("⚠️ Không có dữ liệu phân quyền");
            return;
        }

        // Lọc ra chỉ menu (loại bỏ các quyền chức năng)
        const menuList = filterMainMenus(permissions);

        // Render ra UI
        renderMenu(menuList);

    } catch (err) {
        console.error("❌ Lỗi khi load menu:", err);
    }
};

// 🔹 FILTER MAIN MENUS
const filterMainMenus = (permissions = []) => {
    const seen = new Set();
    return permissions.filter(p => {
        // chỉ lấy các dòng có menu_url (tức là menu chính)
        const isMenu = p.menu_url && !p.function_url;
        const isUnique = !seen.has(p.menu_url);
        if (isMenu && isUnique) {
            seen.add(p.menu_url);
            return true;
        }
        return false;
    });
};

// 🔹 RENDER MENU TO UI
const renderMenu = (menus = []) => {
    const menuContainer = document.querySelector("#view-menu");
    if (!menuContainer) return;

    const menuHTML = menus.map(item => `
    <div class="col-4 col-sm-3 col-md-2 col-lg-2 d-flex flex-column align-items-center mb-3">
      <a href="${item.menu_url}" class="text-decoration-none text-center w-100">
        <div class="bg-white rounded-4 p-3 shadow-sm d-flex justify-content-center align-items-center mx-auto transition-transform transform-hover" 
             style="width: 80px; height: 80px;">
          <img src="https://img.icons8.com/ios-filled/100/000000/${item.icon_url || '/new-moon.png'}" 
               alt="${item.menu_name}" 
               class="img-fluid transition-transform transform-hover" />
        </div>
        <div class="mt-2 text-dark fw-semibold transition-transform transform-hover text-truncate" 
             style="min-height: 40px; max-width: 100%;">
          ${item.menu_name}
        </div>
      </a>
    </div>
  `).join("");

    menuContainer.innerHTML = menuHTML;
};
