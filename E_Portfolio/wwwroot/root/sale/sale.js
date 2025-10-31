// POS System - Sale Management with Multiple Orders
let allProducts = []; // Store all products from API
let orders = {}; // Object to store orders by table/takeaway key
let currentOrderKey = null; // Current active order key (e.g., "table-1", "takeaway")
let tableCount = 20; // Number of tables

// Persistent storage keys
const STORAGE_KEYS = {
	orders: 'pos_orders_v1',
	currentOrderKey: 'pos_current_order_key_v1'
};

function saveOrdersToStorage() {
	try {
		const payload = JSON.stringify(orders);
		localStorage.setItem(STORAGE_KEYS.orders, payload);
		if (currentOrderKey) {
			localStorage.setItem(STORAGE_KEYS.currentOrderKey, currentOrderKey);
		}
	} catch (e) {
		console.warn('Không thể lưu đơn hàng vào bộ nhớ trình duyệt:', e);
	}
}

function removeOrderFromStorage(orderKey) {
	try {
		const stored = localStorage.getItem(STORAGE_KEYS.orders);
		if (!stored) return;
		const parsed = JSON.parse(stored) || {};
		delete parsed[orderKey];
		localStorage.setItem(STORAGE_KEYS.orders, JSON.stringify(parsed));
		const storedCurrent = localStorage.getItem(STORAGE_KEYS.currentOrderKey);
		if (storedCurrent === orderKey) {
			localStorage.removeItem(STORAGE_KEYS.currentOrderKey);
		}
	} catch (e) {
		console.warn('Không thể xóa đơn hàng khỏi bộ nhớ trình duyệt:', e);
	}
}

function loadOrdersFromStorage() {
	try {
		const stored = localStorage.getItem(STORAGE_KEYS.orders);
		if (stored) {
			const parsed = JSON.parse(stored);
			if (parsed && typeof parsed === 'object') {
				orders = parsed;
			}
		}
		const storedCurrent = localStorage.getItem(STORAGE_KEYS.currentOrderKey);
		if (storedCurrent && orders[storedCurrent]) {
			currentOrderKey = storedCurrent;
		}
	} catch (e) {
		console.warn('Không thể tải đơn hàng từ bộ nhớ trình duyệt:', e);
	}
}

// Order structure
function createNewOrderData(orderType, tableNumber = null) {
    return {
        cart: [],
        cartCount: 0,
        subtotal: 0,
        discount: 0,
        total: 0,
        orderType: orderType, // 'table' or 'takeaway'
        tableNumber: tableNumber,
        customerInfo: '',
        customerPaid: 0,
        change: 0,
        timestamp: new Date()
    };
}

// Get order key
function getOrderKey(orderType, tableNumber = null) {
    return orderType === 'table' ? `table-${tableNumber}` : 'takeaway';
}

// Get order display name
function getOrderDisplayName(orderType, tableNumber = null) {
    if (orderType === 'table') {
        return `Bàn ${tableNumber}`;
    }
    return 'Mang về';
}

// Initialize on page load
$(document).ready(function () {
    loadOrdersFromStorage();
    loadProducts();
    loadCategories();
    setupEventListeners();
    generateTableGrid();

    // Restore tabs for stored orders
    const orderKeys = Object.keys(orders);
    if (orderKeys.length > 0) {
    	orderKeys.forEach(key => {
    		const meta = orders[key];
    		if (meta && !document.getElementById(`order-tab-${key}`)) {
    			createOrderTab(key, meta.orderType, meta.tableNumber);
    		}
    	});
    	// Select last active or first available
    	if (currentOrderKey && orders[currentOrderKey]) {
    		$(`#order-tab-${currentOrderKey}`).tab('show');
    		$(`#order-${currentOrderKey}`).addClass('show active');
    		updateCart(currentOrderKey);
    	} else {
    		const first = orderKeys[0];
    		currentOrderKey = first;
    		$(`#order-tab-${first}`).tab('show');
    		$(`#order-${first}`).addClass('show active');
    		updateCart(first);
    	}
    } else {
    	selectTable('table', 1);
    }
    // Keyboard shortcuts
    $(document).on('keydown', function (e) {
        if (e.key === 'F9' && currentOrderKey) {
            e.preventDefault();
            processPayment(currentOrderKey);
        }
    });
});

// Generate table grid in dropdown
function generateTableGrid() {
    const $tableGrid = $('#tableGrid');
    let html = '';
    
    for (let i = 1; i <= tableCount; i++) {
        const orderKey = `table-${i}`;
        const hasOrder = orders[orderKey] && orders[orderKey].cart.length > 0;
        const badgeClass = hasOrder ? 'badge-danger' : 'badge-secondary';
        const badgeText = hasOrder ? orders[orderKey].cartCount : '';
        
        html += `
            <button class="table-btn ${hasOrder ? 'has-order' : ''}" onclick="selectTable('table', ${i})" title="Bàn ${i}">
                <span class="table-number">${i}</span>
                ${hasOrder ? `<span class="badge ${badgeClass}">${badgeText}</span>` : ''}
            </button>
        `;
    }
    
    $tableGrid.html(html);
}

// Select table or takeaway
function selectTable(orderType, tableNumber = null) {
    const orderKey = getOrderKey(orderType, tableNumber);
    
    // Create order if doesn't exist
    if (!orders[orderKey]) {
        orders[orderKey] = createNewOrderData(orderType, tableNumber);
    }
    
    // Check if tab already exists
    const isNewTab = $(`#order-tab-${orderKey}`).length === 0;
    if (isNewTab) {
        createOrderTab(orderKey, orderType, tableNumber);
    }
    
    // Switch to tab and update current order key
    currentOrderKey = orderKey;
    
    // Hide all tabs first
    $('#orderTabs .nav-link').removeClass('active');
    $('#orderTabsContent .tab-pane').removeClass('show active');
    
    // Show selected tab
    $(`#order-tab-${orderKey}`).addClass('active');
    $(`#order-${orderKey}`).addClass('show active');
    
    // Hide empty state
    $('#emptyState').hide();
    
    // Update cart display
    if (!isNewTab) {
        updateCart(orderKey);
    }

	// Persist selection
	saveOrdersToStorage();
}

// Create order tab
function createOrderTab(orderKey, orderType, tableNumber) {
    const displayName = getOrderDisplayName(orderType, tableNumber);
    const icon = orderType === 'table' ? 'bi-table' : 'bi-bag-check';
    
    // Create tab button
    const tabHtml = `
    <li class="nav-item position-relative" role="presentation">
        <button class="nav-link small d-flex align-items-center justify-content-between gap-2" 
                id="order-tab-${orderKey}" data-bs-toggle="tab" 
                data-bs-target="#order-${orderKey}" type="button" role="tab" data-order-key="${orderKey}">
            
            <span class="d-flex align-items-center">
                <i class="${icon} me-1"></i>
                <span class="order-tab-name">${displayName}</span>
            </span>
            
            <i class="bi bi-x-circle-fill text-danger small opacity-75 hover-opacity-100" 
               role="button"
               data-bs-toggle="tooltip"
               title="Đóng đơn"
               onclick="closeOrderTab('${orderKey}', event)">
            </i>
        </button>
    </li>`;
    $('#orderTabs').append(tabHtml);

    
    // Create tab content
    const contentHtml = `
        <div class="tab-pane fade h-100 d-flex flex-column" id="order-${orderKey}" role="tabpanel">
            <!-- Cart Header -->
            <div class="p-2 bg-white border-bottom">
                <div class="d-flex justify-content-between align-items-center">
                    <span class="fw-semibold">
                        <i class="bi bi-cart-check me-1"></i>Giỏ hàng - ${displayName}
                    </span>
                    <span class="badge bg-primary" id="cartCount-${orderKey}">0</span>
                </div>
            </div>

            <!-- Cart Items -->
            <div class="flex-grow-1 p-3 overflow-auto" style="max-height: calc(100vh - 550px);" id="cartItems-${orderKey}">
                <div class="text-center text-muted py-5">
                    <i class="bi bi-cart-x fs-1 d-block mb-3"></i>
                    <p>Chưa có sản phẩm trong giỏ hàng</p>
                </div>
            </div>

            <!-- Customer Info & Payment Details -->
            <div class="p-3 bg-white border-top">
                <div class="mb-2">
                    <label class="form-label small mb-1 fw-semibold">Khách hàng</label>
                    <div class="input-group input-group-sm">
                        <input type="text" class="form-control" id="customerInfo-${orderKey}" placeholder="Tên hoặc SĐT khách hàng">
                        <button class="btn btn-outline-secondary" type="button">
                            <i class="bi bi-person-plus"></i>
                        </button>
                    </div>
                </div>
                
                <div class="row g-2 mb-2">
                 <div class="col-6">
                        <label class="form-label small mb-1 fw-semibold">Giảm giá</label>
                        <p class="mb-0 fw-bold fs-6 text-danger" id="discount-${orderKey}">0 đ</p>
                    </div>
                    <div class="col-6">
                        <label class="form-label small mb-1 fw-semibold">Tổng tiền</label>
                        <p class="mb-0 fw-bold fs-6 text-primary" id="subtotal-${orderKey}">0 đ</p>
                    </div>
                </div>

                <!-- Customer Paid Section -->
                <div class="mb-2">
                    <label class="form-label small mb-1 fw-semibold">Khách đưa</label>
                    <input type="number" class="form-control form-control-sm" id="customerPaid-${orderKey}" 
                           placeholder="Nhập số tiền khách đưa" min="0" onchange="calculateChange('${orderKey}')">
                    <div class="quick-amount-buttons mt-2" id="quickAmounts-${orderKey}">
                        <!-- Quick amount buttons will be generated dynamically -->
                    </div>
                </div>

                <!-- Change Display -->
                <div class="mb-3 p-2 bg-light rounded" id="changeDisplay-${orderKey}" style="display: none;">
                    <div class="d-flex justify-content-between align-items-center">
                        <span class="small fw-semibold">Tiền thừa:</span>
                        <span class="fw-bold text-success fs-5" id="change-${orderKey}">0 đ</span>
                    </div>
                </div>
            </div>

            <!-- Total & Payment -->
            <div class="p-3 bg-white border-top">
                <div class="d-grid gap-2">
                    <button class="btn btn-success btn-lg fw-bold" id="btnPayment-${orderKey}" onclick="processPayment('${orderKey}')">
                        <i class="bi bi-credit-card me-2"></i>Thanh toán (F9)
                    </button>
                    <div class="row g-2">
                        <div class="col-6">
                            <button class="btn btn-outline-secondary w-100" onclick="clearCart('${orderKey}')">
                                <i class="bi bi-trash me-1"></i>Xóa đơn
                            </button>
                        </div>
                        <div class="col-6">
                            <button class="btn btn-outline-dark w-100" onclick="printOrder('${orderKey}')">
                                <i class="bi bi-printer me-1"></i>In hóa đơn
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
    $('#orderTabsContent').append(contentHtml);
    
    updateCart(orderKey);
}

// Setup event listeners
function setupEventListeners() {
    // Search product
    $('#searchProduct').on('input', function () {
        const searchTerm = $(this).val();

        // Client-side filter for better UX (instant)
        if (searchTerm.trim() === '') {
            renderProducts(allProducts, '#productGrid');
        } else {
            const filtered = allProducts.filter(p =>
                p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                p.code.toLowerCase().includes(searchTerm.toLowerCase())
            );
            renderProducts(filtered, '#productGrid');
        }
    });

    // Category filter
    $('#categoryFilter').on('change', function () {
        const categoryId = $(this).val();
        filterByCategory(categoryId);
    });

    // Tab change events
    $('#all-tab').on('shown.bs.tab', function () {
        renderProducts(allProducts, '#productGrid');
    });

    $('#combo-tab').on('shown.bs.tab', function () {
        const comboProducts = allProducts.filter(p => p.CategoryId === 1);
        renderProducts(comboProducts, '#comboGrid');
    });

    $('#drink-tab').on('shown.bs.tab', function () {
        const drinkProducts = allProducts.filter(p => p.CategoryId === 2 || p.CategoryId === 4 || p.CategoryId === 5);
        renderProducts(drinkProducts, '#drinkGrid');
    });

    // Customer info change listener
    $(document).on('change', '[id^="customerInfo-"]', function () {
        const orderKey = $(this).attr('id').replace('customerInfo-', '');
        if (orders[orderKey]) {
            orders[orderKey].customerInfo = $(this).val();
        }
    });

    // Order tab click listener
    $(document).on('click', '[id^="order-tab-"]', function (e) {
        // Don't trigger if clicking the close button
        if ($(e.target).closest('.bi-x-circle-fill').length > 0) {
            return;
        }
        
        const orderKey = $(this).attr('data-order-key');
        
        // Hide all tabs first
        $('#orderTabs .nav-link').removeClass('active');
        $('#orderTabsContent .tab-pane').removeClass('show active d-flex');
        
        // Show selected tab
        $(this).addClass('active');
        $(`#order-${orderKey}`).addClass('show active d-flex');
        
        // Update current order key
        currentOrderKey = orderKey;
        
        // Hide empty state
        $('#emptyState').hide();
        
        // Refresh cart display
        if (orders[orderKey]) {
            updateCart(orderKey);
        }
    });
}

// Load products from API
async function loadProducts() {
    try {
        await apiHelper.post('/Sale/Select-Menus', {},
            function (res) {
                allProducts = Array.isArray(res.data) ? res.data : [];
                renderProducts(allProducts, '#productGrid');
            },
            function (err) {
                console.error("Lỗi tải sản phẩm:", err);
                toastr.warning('Không thể tải sản phẩm, thử lại sau.');
            });
    } catch (error) {
        console.error('Error loading products:', error);
    }
}

// Load categories from API
async function loadCategories() {
    try {
        await apiHelper.get('/Sale/Select-Categories', {},
            function (res) {
                var categories = Array.isArray(res.data) ? res.data : [];
                renderCategoryFilter(categories);
            },
            function (err) {
                console.error("Lỗi tải danh mục:", err);
            });
    } catch (error) {
        console.error('Error loading categories:', error);
    }
}

// Render products to grid
function renderProducts(products, targetElement) {
    const $grid = $(targetElement);

    if (!products || products.length === 0) {
        $grid.html(`
            <div class="col-12 text-center py-5 text-muted">
                <i class="bi bi-box-seam fs-1 d-block mb-3"></i>
                <p>Không có sản phẩm</p>
            </div>
        `);
        return;
    }

    let html = '';
    products.forEach(product => {
        const imageUrl = product.ImageUrl || 'https://via.placeholder.com/200x200?text=' + encodeURIComponent(product.Name);
        const hasSale = product.SalePrice && product.SalePrice > 0;
        const displayPrice = hasSale ? product.SalePrice : product.Price;

        html += `
            <div class="col">
                <div class="card h-100 shadow-sm border-0 product-card" style="cursor: pointer;" onclick="addToCartById(${product.Id})">
                    <div class="position-relative">
                        <img src="${imageUrl}" class="card-img-top h-130px" alt="${product.Name}" 
                             onerror="this.src='https://via.placeholder.com/200x200?text=No+Image'">
                        ${hasSale ? '<span class="position-absolute top-0 end-0 m-2 badge bg-danger">Sale</span>' : ''}
                        ${product.IsBestSeller ? '<span class="position-absolute top-0 start-0 m-2 badge bg-success">Hot</span>' : ''}
                    </div>
                    <div class="card-body p-2">
                        <h6 class="card-title mb-1 text-truncate small fw-bold" title="${product.Name}">
                            <span class="card-text text-muted small mb-1">${product.Code}</span> - 
                            ${product.Name}
                        </h6>
                        <div class="d-flex flex-column">
                            ${hasSale ? `
                                <small class="text-decoration-line-through text-muted">${formatCurrency(product.Price)}</small>
                                <p class="mb-0 fw-bold text-danger">${formatCurrency(product.SalePrice)}</p>
                            ` : `
                                <p class="mb-0 fw-bold text-primary">${formatCurrency(product.Price)}</p>
                            `}
                        </div>
                        ${product.Stock > 0 ? '' : '<span class="badge bg-secondary small">Hết hàng</span>'}
                    </div>
                </div>
            </div>
        `;
    });

    $grid.html(html);
}

// Render category filter dropdown
function renderCategoryFilter(categories) {
    const $select = $('#categoryFilter');
    let html = '<option value="">Tất cả danh mục</option>';

    categories.forEach(cat => {
        html += `<option value="${cat.category_id}">${cat.category_name}</option>`;
    });

    $select.html(html);
}

// Filter products by search term
function filterProducts(searchTerm) {
    $('.product-card').each(function () {
        const productName = $(this).find('.card-title').text().toLowerCase();
        const productCode = $(this).find('.card-text').text().toLowerCase();

        if (productName.includes(searchTerm) || productCode.includes(searchTerm)) {
            $(this).closest('.col').show();
        } else {
            $(this).closest('.col').hide();
        }
    });
}

// Filter by category
async function filterByCategory(categoryId) {
    if (!categoryId) {
        renderProducts(allProducts, '#productGrid');
        return;
    }

    try {
        await apiHelper.post('/Sale/Select-Menus', { CategoryId: categoryId },
            function (res) {
                var products = Array.isArray(res.data) ? res.data : [];
                renderProducts(products, '#productGrid');
            },
            function (err) {
                console.error("Lỗi lọc sản phẩm theo danh mục:", err);
                toastr.warning('Không thể lọc sản phẩm.');
            });
    } catch (error) {
        console.error('Error filtering by category:', error);
    }
}

// Add product to cart by ID
function addToCartById(productId) {
    if (!currentOrderKey) {
        showToast('warning', 'Vui lòng chọn bàn hoặc mang về trước');
        return;
    }

    const product = allProducts.find(p => p.Id === productId);

    if (!product) {
        showToast('error', 'Không tìm thấy sản phẩm');
        return;
    }

    if (product.stock <= 0) {
        showToast('warning', 'Sản phẩm đã hết hàng');
        return;
    }

    addToCart(product, currentOrderKey);
}

// Add product to cart
function addToCart(product, orderKey) {
    const order = orders[orderKey];
    const existingItem = order.cart.find(item => item.id === product.Id);

    if (existingItem) {
        existingItem.quantity++;
    } else {
        order.cart.push({
            id: product.Id,
            name: product.Name,
            code: product.Code,
            price: product.Price,
            salePrice: product.SalePrice,
            quantity: 1
        });
    }

    updateCart(orderKey);
    generateTableGrid(); // Update table grid to show badge
    showToast('success', 'Đã thêm vào giỏ hàng!');

	// Persist change
	saveOrdersToStorage();
}

// Update cart display
function updateCart(orderKey) {
    const order = orders[orderKey];
    const $cartItems = $(`#cartItems-${orderKey}`);
    
    order.cartCount = order.cart.reduce((sum, item) => sum + item.quantity, 0);
    order.subtotal = 0;
    order.discount = 0;

    if (order.cart.length === 0) {
        $cartItems.html(`
            <div class="text-center text-muted py-5">
                <i class="bi bi-cart-x fs-1 d-block mb-3"></i>
                <p>Chưa có sản phẩm trong giỏ hàng</p>
            </div>
        `);
    } else {
        let html = '';
        order.cart.forEach((item, index) => {
            const itemPrice = item.salePrice || item.price;
            const itemTotal = itemPrice * item.quantity;
            order.subtotal += itemTotal;

            if (item.salePrice) {
                order.discount += (item.price - item.salePrice) * item.quantity;
            }

            html += `
                <div class="card mb-2 shadow-sm border-0">
                    <div class="card-body p-2">
                        <div class="d-flex justify-content-between align-items-start mb-2">
                            <div class="flex-grow-1">
                                <h6 class="mb-0 small fw-bold"><small class="text-muted">${item.code}</small> - ${item.name}</h6>
                            </div>
                        </div>
                        <div class="d-flex justify-content-between align-items-center">
                            <div class="btn-group btn-group-sm" role="group">
                                <button type="button" class="btn btn-outline-secondary" onclick="decreaseQuantity('${orderKey}', ${index})">
                                    <i class="bi bi-dash"></i>
                                </button>
                                <button type="button" class="btn btn-outline-secondary disabled" style="min-width: 50px;">
                                    ${item.quantity}
                                </button>
                                <button type="button" class="btn btn-outline-secondary" onclick="increaseQuantity('${orderKey}', ${index})">
                                    <i class="bi bi-plus"></i>
                                </button>
                            </div>
                            <div class="text-end">
                                ${item.salePrice ? `<small class="text-decoration-line-through text-muted d-block">${formatCurrency(item.price)}</small>` : ''}
                                <span class="fw-bold ${item.salePrice ? 'text-danger' : 'text-primary'}">${formatCurrency(itemTotal)}</span>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        });
        $cartItems.html(html);
    }

    order.total = order.subtotal;

    // Update summary
    $(`#cartCount-${orderKey}`).text(order.cartCount);
    $(`#subtotal-${orderKey}`).text(formatCurrency(order.subtotal));
    $(`#discount-${orderKey}`).text(formatCurrency(order.discount));
    $(`#totalAmount-${orderKey}`).text(formatCurrency(order.total));

    // Enable/disable payment button
    $(`#btnPayment-${orderKey}`).prop('disabled', order.cart.length === 0);
    
    // Update tab badge
    updateOrderTabBadge(orderKey);
    
    // Generate quick amount suggestions
    generateQuickAmounts(orderKey);
    
    // Recalculate change if customer paid is already entered
    calculateChange(orderKey);

	// Persist after recalculation
	saveOrdersToStorage();
}

// Generate quick amount suggestions
function generateQuickAmounts(orderKey) {
    const order = orders[orderKey];
    const total = order.total;
    const $quickAmounts = $(`#quickAmounts-${orderKey}`);
    
    if (total === 0) {
        $quickAmounts.html('');
        return;
    }
    
    // Generate smart suggestions
    const suggestions = [];
    
    // Exact amount
    suggestions.push(total);
    
    // Round up to nearest 1000, 5000, 10000, 50000, 100000
    const roundings = [1000, 5000, 10000, 20000, 30000, 50000, 100000, 150000, 200000, 500000];
    for (let rounding of roundings) {
        const rounded = Math.ceil(total / rounding) * rounding;
        if (rounded > total && !suggestions.includes(rounded)) {
            suggestions.push(rounded);
            if (suggestions.length >= 5) break;
        }
    }
    
    // Limit to 6 suggestions
    const finalSuggestions = suggestions.slice(0, 5);
    
    let html = '';
    finalSuggestions.forEach(amount => {
        html += `
            <span class="badge bg-success me-1" onclick="selectQuickAmount('${orderKey}', ${amount})">
                ${formatCurrency(amount)}
            </span>
        `;
    });
    
    $quickAmounts.html(html);
}

// Select quick amount
function selectQuickAmount(orderKey, amount) {
    $(`#customerPaid-${orderKey}`).val(amount);
    calculateChange(orderKey);
}

// Calculate change
function calculateChange(orderKey) {
    const order = orders[orderKey];
    const customerPaid = parseFloat($(`#customerPaid-${orderKey}`).val()) || 0;
    const total = order.total;
    
    if (customerPaid > 0) {
        const change = customerPaid - total;
        order.customerPaid = customerPaid;
        order.change = change;
        
        if (change >= 0) {
            $(`#change-${orderKey}`).text(formatCurrency(change));
            $(`#changeDisplay-${orderKey}`).show();
        } else {
            $(`#change-${orderKey}`).text('Chưa đủ');
            $(`#changeDisplay-${orderKey}`).show();
        }
    } else {
        $(`#changeDisplay-${orderKey}`).hide();
    }
}

// Increase quantity
function increaseQuantity(orderKey, itemIndex) {
    orders[orderKey].cart[itemIndex].quantity++;
    updateCart(orderKey);
    generateTableGrid();
}

// Decrease quantity
function decreaseQuantity(orderKey, itemIndex) {
    if (orders[orderKey].cart[itemIndex].quantity > 1) {
        orders[orderKey].cart[itemIndex].quantity--;
    } else {
        removeFromCart(orderKey, itemIndex);
    }
    updateCart(orderKey);
    generateTableGrid();
}

// Remove item from cart
function removeFromCart(orderKey, itemIndex) {
    orders[orderKey].cart.splice(itemIndex, 1);
    updateCart(orderKey);
    generateTableGrid();
    showToast('info', 'Đã xóa sản phẩm khỏi giỏ hàng');
}

// Clear cart
function clearCart(orderKey) {
    Swal.fire({
        title: 'Xác nhận xóa đơn hàng?',
        text: 'Bạn có chắc muốn xóa toàn bộ đơn hàng này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Xóa đơn',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
			orders[orderKey].cart = [];
            orders[orderKey].customerInfo = '';
            orders[orderKey].customerPaid = 0;
            $(`#customerInfo-${orderKey}`).val('');
            $(`#customerPaid-${orderKey}`).val('');
            updateCart(orderKey);
            generateTableGrid();
            showToast('success', 'Đã xóa đơn hàng');
			saveOrdersToStorage();
        }
    });
}

// Close order tab
function closeOrderTab(orderKey, event) {
    if (event) {
        event.stopPropagation();
    }
    
    const order = orders[orderKey];
    
    // Check if order has items
    if (order && order.cart.length > 0) {
        Swal.fire({
            title: 'Đóng đơn hàng?',
            text: 'Đơn hàng này có sản phẩm. Bạn có chắc muốn đóng?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Đóng đơn',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                removeOrderTab(orderKey);
            }
        });
    } else {
        removeOrderTab(orderKey);
    }
}

// Remove order tab
function removeOrderTab(orderKey) {
    // Remove tab and content
    $(`#order-tab-${orderKey}`).parent().remove();
    $(`#order-${orderKey}`).remove();
    
    // Remove from orders object
    delete orders[orderKey];
	removeOrderFromStorage(orderKey);
    
    // Update table grid
    generateTableGrid();
    
    // Switch to first tab if exists, otherwise show empty state
    const firstTab = $('#orderTabs .nav-item:first button');
    if (firstTab.length > 0) {
        firstTab.tab('show');
        currentOrderKey = firstTab.attr('data-order-key');
    } else {
        currentOrderKey = null;
        $('#emptyState').show();
    }
    
    showToast('info', 'Đã đóng đơn hàng');
	
	// Persist selection change
	saveOrdersToStorage();
}

// Update order tab badge
function updateOrderTabBadge(orderKey) {
    const order = orders[orderKey];
    const $tab = $(`#order-tab-${orderKey}`);
    
    // Remove existing badge
    $tab.find('.badge').remove();
    
    // Add badge if cart has items
    if (order.cartCount > 0) {
        $tab.find('.order-tab-name').after(`<span class="badge bg-success ms-1">${order.cartCount}</span>`);
    }
}

// Print order (without payment)
function printOrder(orderKey) {
    const order = orders[orderKey];
    
    if (order.cart.length === 0) {
        showToast('warning', 'Chưa có sản phẩm để in');
        return;
    }
    
    // Generate order data for printing
    const orderData = {
        orderNumber: generateOrderNumber(),
        orderType: order.orderType,
        tableNumber: order.tableNumber,
        customerName: order.customerInfo || 'Khách lẻ',
        items: order.cart,
        subtotal: order.subtotal,
        discount: order.discount,
        total: order.total,
        createdAt: new Date().toISOString()
    };
    
    printReceipt(orderData);
}

// Process payment
function processPayment(orderKey) {
    const order = orders[orderKey];
    
    if (order.cart.length === 0) {
        showToast('warning', 'Chưa có sản phẩm để thanh toán');
        return;
    }

    Swal.fire({
        title: 'Thanh toán đơn hàng',
        html: `
            <div class="text-start">
                <div class="mb-3">
                    <label class="form-label fw-bold">Tổng tiền: <span class="text-primary">${formatCurrency(order.total)}</span></label>
                </div>
                <div class="mb-3">
                    <label class="form-label">Phương thức thanh toán</label>
                    <select class="form-select" id="paymentMethod">
                        <option value="cash">Tiền mặt</option>
                        <option value="card">Thẻ</option>
                        <option value="transfer">Chuyển khoản</option>
                        <option value="momo">MoMo</option>
                    </select>
                </div>
            </div>
        `,
        showCancelButton: true,
        confirmButtonColor: '#198754',
        cancelButtonColor: '#6c757d',
        confirmButtonText: '<i class="bi bi-check-circle me-1"></i> Xác nhận thanh toán',
        cancelButtonText: 'Hủy',
        width: '500px'
    }).then((result) => {
        if (result.isConfirmed) {
            completePayment(orderKey);
        }
    });
}

// Complete payment
async function completePayment(orderKey) {
    const order = orders[orderKey];
    const customerInfo = $(`#customerInfo-${orderKey}`).val();
    const paymentMethod = $('#paymentMethod').val();

    const orderData = {
        orderNumber: generateOrderNumber(),
        orderType: order.orderType,
        tableNumber: order.tableNumber,
        customerName: customerInfo || 'Khách lẻ',
        customerPhone: '',
        items: order.cart,
        subtotal: order.subtotal,
        discount: order.discount,
        total: order.total,
        customerPaid: order.customerPaid,
        change: order.change,
        paymentMethod: paymentMethod,
        status: 'Completed',
    };
    console.log(JSON.stringify(orderData, null, 2));

    try {
        // Send order to API
        await apiHelper.post('/Sale/Update-Order-Payment', orderData,
            function (res) {
                // Success
                const finalOrderNumber = res.orderNumber || orderData.orderNumber;

                Swal.fire({
                    title: 'Thanh toán thành công!',
                    html: `
                        <div class="text-center">
                            <i class="bi bi-check-circle-fill text-success" style="font-size: 4rem;"></i>
                            <p class="mt-3">Mã đơn hàng: <strong>${finalOrderNumber}</strong></p>
                            ${order.orderType === 'table' && order.tableNumber ? `<p>Bàn số: <strong>${order.tableNumber}</strong></p>` : ''}
                            <p>Tổng tiền: <strong class="text-primary">${formatCurrency(order.total)}</strong></p>
                            ${order.customerPaid > 0 ? `<p>Khách đưa: <strong>${formatCurrency(order.customerPaid)}</strong></p>` : ''}
                            ${order.change > 0 ? `<p>Tiền thừa: <strong class="text-success">${formatCurrency(order.change)}</strong></p>` : ''}
                        </div>
                    `,
                    icon: 'success',
                    showCancelButton: true,
                    confirmButtonText: '<i class="bi bi-printer me-1"></i> In hóa đơn',
                    cancelButtonText: 'Đóng',
                    confirmButtonColor: '#0d6efd'
                }).then((result) => {
                    if (result.isConfirmed) {
                        printReceipt({ ...orderData, orderNumber: finalOrderNumber });
                    }
                });

                // Clear cart and reset order
				order.cart = [];
                order.customerInfo = '';
                order.customerPaid = 0;
                order.change = 0;
                $(`#customerInfo-${orderKey}`).val('');
                $(`#customerPaid-${orderKey}`).val('');
                updateCart(orderKey);
                generateTableGrid();
				// Remove this order from persistent storage after successful payment
				removeOrderFromStorage(orderKey);
				saveOrdersToStorage();
            },
            function (err) {
                console.error("Lỗi tạo đơn hàng:", err);
                toastr.error('Không thể tạo đơn hàng, vui lòng thử lại.');
            });
    } catch (error) {
        console.error('Error completing payment:', error);
        toastr.error('Có lỗi xảy ra khi thanh toán.');
    }
}

// Generate order number
function generateOrderNumber() {
    const now = new Date();
    const timestamp = now.getTime();
    return `HD${timestamp.toString().slice(-8)}`;
}

// Print receipt
function printReceipt(orderData) {
    try {
        const templatePath = '/root/sale/receipt-template.html';
        fetch(templatePath, { cache: 'no-cache' })
            .then(res => res.text())
            .then(html => {
                const rendered = renderReceiptHtml(html, orderData);
                const printWindow = window.open('', '_blank', 'width=400,height=700');
                if (!printWindow) {
                    toastr.warning('Trình duyệt chặn cửa sổ in. Vui lòng cho phép pop-up.');
                    return;
                }
                printWindow.document.open();
                printWindow.document.write(rendered);
                printWindow.document.close();
                showToast('info', 'Đang in hóa đơn...');
            })
            .catch(err => {
                console.error('Lỗi tải template hóa đơn:', err);
                toastr.error('Không tải được template hóa đơn.');
            });
    } catch (e) {
        console.error('Lỗi in hóa đơn:', e);
        toastr.error('Không thể in hóa đơn.');
    }
}

function renderReceiptHtml(templateHtml, orderData) {
    const nf = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' });
    const fmt = (v) => nf.format(v || 0);
    const fmtDate = (iso) => {
        try {
            const d = iso ? new Date(iso) : new Date();
            return d.toLocaleString('vi-VN');
        } catch { return new Date().toLocaleString('vi-VN'); }
    };

    const tableInfo = (orderData.orderType === 'table' && orderData.tableNumber)
        ? `<div><span>Bàn:</span><span><strong>${orderData.tableNumber}</strong></span></div>`
        : '';

    const itemsHtml = (orderData.items || []).map(it => {
        const unit = (it.salePrice ?? it.price ?? it.UnitPrice ?? it.SalePrice ?? it.UnitPrice) || 0;
        const qty = it.quantity ?? it.Quantity ?? 1;
        const name = it.name ?? it.ProductName ?? '';
        const lineTotal = unit * qty;
        return `
            <tr>
                <td>${escapeHtml(name)}</td>
                <td class="qty">${qty}</td>
                <td class="price">${fmt(lineTotal)}</td>
            </tr>
        `;
    }).join('');

    const paymentInfo = (() => {
        const pm = orderData.paymentMethod ? `<tr class="total-row"><td class="label">Thanh toán</td><td></td><td class="price">${escapeHtml(orderData.paymentMethod)}</td></tr>` : '';
        const paid = orderData.customerPaid ? `<tr class="total-row"><td class="label">Khách đưa</td><td></td><td class="price">${fmt(orderData.customerPaid)}</td></tr>` : '';
        const change = orderData.change ? `<tr class="total-row"><td class="label">Tiền thừa</td><td></td><td class="price">${fmt(orderData.change)}</td></tr>` : '';
        return pm + paid + change;
    })();

    // Cashier name
    const cashierName = escapeHtml(orderData.createdBy || orderData.cashierName || 'Thu ngân');

    // Watermark: prefer image if provided, else fallback to text brand
    const watermarkHtml = (() => {
        const wmUrl = orderData.watermarkUrl;
        const brand = escapeHtml(orderData.brand || 'Thanh Ha New City');
        if (wmUrl) {
            return `<img src="${wmUrl}" alt="watermark">`;
        }
        return `<div class="wm-text">${brand}</div>`;
    })();

    // VietQR for VPBank 2227036888
    const bankShort = 'vpbank';
    const accountNo = '2227036888';
    const amount = Math.round(orderData.total || 0);
    const addInfo = encodeURIComponent(`Thanh toan ${orderData.orderNumber || ''}`.trim());
    const vietQrUrl = `https://img.vietqr.io/image/${bankShort}-${accountNo}-compact.png?amount=${amount}&addInfo=${addInfo}`;
    const qrImg = `<img src="${vietQrUrl}" alt="VietQR VPBank" onerror="this.onerror=null;this.src='https://api.qrserver.com/v1/create-qr-code/?size=180x180&data='+encodeURIComponent('${addInfo} - ${amount}');">`;

    const replaceMap = {
        '{{orderNumber}}': escapeHtml(orderData.orderNumber || ''),
        '{{createdAt}}': escapeHtml(fmtDate(orderData.createdAt)),
        '{{customerName}}': escapeHtml(orderData.customerName || 'Khách lẻ'),
        '{{cashierName}}': cashierName,
        '{{tableInfo}}': tableInfo,
        '{{items}}': itemsHtml,
        '{{subtotal}}': fmt(orderData.subtotal),
        '{{discount}}': fmt(orderData.discount),
        '{{total}}': fmt(orderData.total),
        '{{paymentInfo}}': paymentInfo,
        '{{qrImage}}': qrImg,
        '{{watermark}}': watermarkHtml
    };

    let result = templateHtml;
    for (const key in replaceMap) {
        result = result.split(key).join(replaceMap[key]);
    }
    return result;
}

function escapeHtml(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

// Show toast notification (use toastr for consistency)
function showToast(type, message) {
    switch (type) {
        case 'success':
            toastr.success(message);
            break;
        case 'error':
            toastr.error(message);
            break;
        case 'warning':
            toastr.warning(message);
            break;
        case 'info':
            toastr.info(message);
            break;
        default:
            toastr.info(message);
    }
}

// Hover effect for product cards
$(document).on('mouseenter', '.product-card', function () {
    $(this).addClass('shadow');
}).on('mouseleave', '.product-card', function () {
    $(this).removeClass('shadow').addClass('shadow-sm');
});

