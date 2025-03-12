const commonDateTime = {
    getCurrentDateTimeFormatted: () => {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const seconds = String(now.getSeconds()).padStart(2, '0');

        return `${year}${month}${day} ${hours}${minutes}${seconds}`;
    },
    getCurrentDateTimeFormatFileName: () => {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const seconds = String(now.getSeconds()).padStart(2, '0');

        return `${year}${month}${day}_${hours}${minutes}${seconds}`;
    },
    getCurrentDateFormatted: () => {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    },
    formatDateChart: (date) => {
        const pad = (num) => num.toString().padStart(2, '0');

        const year = date.getFullYear();
        const month = pad(date.getMonth() + 1);
        const day = pad(date.getDate());
        const hours = pad(date.getHours());
        const minutes = pad(date.getMinutes());

        return `${year}-${month}-${day} ${hours}:${minutes}`;
    },
    toDateFromBackendDate: (dateStr) => {
        const matches = dateStr.match(/\d+/);
        if (matches) {
            return parseInt(matches[0]);
        }
        return null;
    },
    initDataTimeRangeSelect: async (elementId, callBack, to_Date = new Date()) => {
        //to_Date = new Date(to_Date.getTime() - 5 * 60 * 1000); // Subtract 5 minutes

        const start = moment().subtract(24, 'hours');
        const end = moment(to_Date);
        const currentDate = moment();
        const currentQuarter = currentDate.quarter();
        const startOfCurrentQuarter = moment().quarter(currentQuarter).startOf('quarter');
        const endOfCurrentQuarter = moment().quarter(currentQuarter).endOf('quarter');
        const startOfPreviousQuarter = moment().quarter(currentQuarter - 1).startOf('quarter');
        const endOfPreviousQuarter = moment().quarter(currentQuarter - 1).endOf('quarter');
        const startOfYear = moment().startOf('year');
        const endOfYear = currentDate.endOf('year');

        function cb(start, end) {
            var startText = start.format('DD/MM/YYYY HH:mm');
            var endText = end.format('DD/MM/YYYY HH:mm');
            $(`#${elementId}`).val(`${startText} - ${endText}`);
        }
        const ranges = {};
        ranges[await commonExtension.getTranslationByKey('24 giờ gần nhất')] = [start, to_Date];
        ranges[await commonExtension.getTranslationByKey('Hôm nay')] = [moment().startOf('day'), to_Date];                               // [moment().startOf('day'), moment().endOf('day')];
        ranges[await commonExtension.getTranslationByKey('Hôm qua')] = [moment().startOf('day').subtract(1, 'days'), moment().endOf('day').subtract(1, 'days')];
        ranges[await commonExtension.getTranslationByKey('7 ngày gần nhất')] = [moment().startOf('day').subtract(6, 'days'), to_Date];   //[moment().startOf('day').subtract(6, 'days'), moment().endOf('day')];
        ranges[await commonExtension.getTranslationByKey('30 ngày gần nhất')] = [moment().startOf('day').subtract(29, 'days'), to_Date]; // [moment().startOf('day').subtract(29, 'days'), moment().endOf('day')];
        ranges[await commonExtension.getTranslationByKey('Tháng này')] = [moment().startOf('month'), to_Date];                         //[moment().startOf('month'), moment().endOf('month')];
        ranges[await commonExtension.getTranslationByKey('Tháng trước')] = [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')];
        ranges[await commonExtension.getTranslationByKey('Quý này')] = [startOfCurrentQuarter, to_Date];                                   //[startOfCurrentQuarter, endOfCurrentQuarter];
        ranges[await commonExtension.getTranslationByKey('Quý trước')] = [startOfPreviousQuarter, endOfPreviousQuarter];
        ranges[await commonExtension.getTranslationByKey('Cả năm')] = [startOfYear, to_Date];
        $(`#${elementId}`).daterangepicker({
            timePicker: true,
            timePicker24Hour: true,
            startDate: start,
            endDate: end,
            locale: {
                format: 'DD/MM/YYYY HH:mm',
                applyLabel: await commonExtension.getTranslationByKey('Áp dụng'),
                cancelLabel: await commonExtension.getTranslationByKey('Hủy'),
                fromLabel: await commonExtension.getTranslationByKey('Từ'),
                toLabel: await commonExtension.getTranslationByKey('Đến'),
                customRangeLabel: await commonExtension.getTranslationByKey('Tùy chỉnh'),
                daysOfWeek: [
                    await commonExtension.getTranslationByKey('CN'),
                    await commonExtension.getTranslationByKey('T2'),
                    await commonExtension.getTranslationByKey('T3'),
                    await commonExtension.getTranslationByKey('T4'),
                    await commonExtension.getTranslationByKey('T5'),
                    await commonExtension.getTranslationByKey('T6'),
                    await commonExtension.getTranslationByKey('T7')
                ],
                monthNames: [
                    await commonExtension.getTranslationByKey('Tháng 1'),
                    await commonExtension.getTranslationByKey('Tháng 2'),
                    await commonExtension.getTranslationByKey('Tháng 3'),
                    await commonExtension.getTranslationByKey('Tháng 4'),
                    await commonExtension.getTranslationByKey('Tháng 5'),
                    await commonExtension.getTranslationByKey('Tháng 6'),
                    await commonExtension.getTranslationByKey('Tháng 7'),
                    await commonExtension.getTranslationByKey('Tháng 8'),
                    await commonExtension.getTranslationByKey('Tháng 9'),
                    await commonExtension.getTranslationByKey('Tháng 10'),
                    await commonExtension.getTranslationByKey('Tháng 11'),
                    await commonExtension.getTranslationByKey('Tháng 12')],
                firstDay: 1,
                //    amPm: ['Sa', 'Ch']
            },

            ranges: ranges
        }, cb);

        cb(moment().startOf('day'), moment(to_Date));

        if (typeof callBack === 'function') {
            callBack();
        }
    },
    initDataTimeRangeWithDefault: async (elementId, initFinishcallBack, initOptionIndex = 0) => {
        var start = moment().subtract(24, 'hours');
        var end = moment();
        const currentDate = moment();
        const currentQuarter = currentDate.quarter();
        const startOfCurrentQuarter = moment().quarter(currentQuarter).startOf('quarter');
        const endOfCurrentQuarter = moment().quarter(currentQuarter).endOf('quarter');
        const startOfPreviousQuarter = moment().quarter(currentQuarter - 1).startOf('quarter');
        const endOfPreviousQuarter = moment().quarter(currentQuarter - 1).endOf('quarter');
        const startOfYear = moment().startOf('year');
        const endOfYear = currentDate.endOf('year');

        // ----------- option -----------
        const ranges = {};
        ranges[await commonExtension.getTranslationByKey('24 giờ gần nhất')] = [start, end];
        ranges[await commonExtension.getTranslationByKey('Hôm nay')] = [moment().startOf('day'), end];
        ranges[await commonExtension.getTranslationByKey('Hôm qua')] = [moment().startOf('day').subtract(1, 'days'), moment().endOf('day').subtract(1, 'days')];
        ranges[await commonExtension.getTranslationByKey('7 ngày gần nhất')] = [moment().startOf('day').subtract(6, 'days'), end];
        ranges[await commonExtension.getTranslationByKey('30 ngày gần nhất')] = [moment().startOf('day').subtract(29, 'days'), end];
        ranges[await commonExtension.getTranslationByKey('Tháng này')] = [moment().startOf('month'), end];
        ranges[await commonExtension.getTranslationByKey('Tháng trước')] = [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')];
        ranges[await commonExtension.getTranslationByKey('Quý này')] = [startOfCurrentQuarter, end];
        ranges[await commonExtension.getTranslationByKey('Quý trước')] = [startOfPreviousQuarter, endOfPreviousQuarter];
        ranges[await commonExtension.getTranslationByKey('Cả năm')] = [startOfYear, end];

        const rangesDate = [];
        rangesDate[0] = { t: await commonExtension.getTranslationByKey('24 giờ gần nhất'), s: start, e: end };
        rangesDate[1] = { t: await commonExtension.getTranslationByKey('Hôm nay'), s: moment().startOf('day'), e: end };
        rangesDate[2] = { t: await commonExtension.getTranslationByKey('Hôm qua'), s: moment().startOf('day').subtract(1, 'days'), e: moment().endOf('day').subtract(1, 'days') };
        rangesDate[3] = { t: await commonExtension.getTranslationByKey('7 ngày gần nhất'), s: moment().startOf('day').subtract(6, 'days'), e: end };
        rangesDate[4] = { t: await commonExtension.getTranslationByKey('30 ngày gần nhất'), s: moment().startOf('day').subtract(29, 'days'), e: end };
        rangesDate[5] = { t: await commonExtension.getTranslationByKey('Tháng này'), s: moment().startOf('month'), e: end };
        rangesDate[6] = { t: await commonExtension.getTranslationByKey('Tháng trước'), s: moment().subtract(1, 'month').startOf('month'), e: moment().subtract(1, 'month').endOf('month') };
        rangesDate[7] = { t: await commonExtension.getTranslationByKey('Quý này'), s: startOfCurrentQuarter, e: end };
        rangesDate[8] = { t: await commonExtension.getTranslationByKey('Quý trước'), s: startOfPreviousQuarter, e: endOfPreviousQuarter };
        rangesDate[9] = { t: await commonExtension.getTranslationByKey('Cả năm'), s: startOfYear, e: end };
        // ---------------------------------
        if (initOptionIndex > rangesDate.length || initOptionIndex < 0) {
            initOptionIndex = 0;
        }
        start = rangesDate[initOptionIndex].s;
        end = rangesDate[initOptionIndex].e;

        function cb(start, end) {
            var startText = start.format('DD/MM/YYYY HH:mm');
            var endText = end.format('DD/MM/YYYY HH:mm');
            $(`#${elementId}`).val(`${startText} - ${endText}`);
        }

        $(`#${elementId}`).daterangepicker({
            timePicker: true,
            timePicker24Hour: true,
            startDate: start,
            endDate: end,
            locale: {
                format: 'DD/MM/YYYY HH:mm',
                applyLabel: await commonExtension.getTranslationByKey('Áp dụng'),
                cancelLabel: await commonExtension.getTranslationByKey('Hủy'),
                fromLabel: await commonExtension.getTranslationByKey('Từ'),
                toLabel: await commonExtension.getTranslationByKey('Đến'),
                customRangeLabel: await commonExtension.getTranslationByKey('Tùy chỉnh'),
                daysOfWeek: [
                    await commonExtension.getTranslationByKey('CN'),
                    await commonExtension.getTranslationByKey('T2'),
                    await commonExtension.getTranslationByKey('T3'),
                    await commonExtension.getTranslationByKey('T4'),
                    await commonExtension.getTranslationByKey('T5'),
                    await commonExtension.getTranslationByKey('T6'),
                    await commonExtension.getTranslationByKey('T7')
                ],
                monthNames: [
                    await commonExtension.getTranslationByKey('Tháng 1'),
                    await commonExtension.getTranslationByKey('Tháng 2'),
                    await commonExtension.getTranslationByKey('Tháng 3'),
                    await commonExtension.getTranslationByKey('Tháng 4'),
                    await commonExtension.getTranslationByKey('Tháng 5'),
                    await commonExtension.getTranslationByKey('Tháng 6'),
                    await commonExtension.getTranslationByKey('Tháng 7'),
                    await commonExtension.getTranslationByKey('Tháng 8'),
                    await commonExtension.getTranslationByKey('Tháng 9'),
                    await commonExtension.getTranslationByKey('Tháng 10'),
                    await commonExtension.getTranslationByKey('Tháng 11'),
                    await commonExtension.getTranslationByKey('Tháng 12')],
                firstDay: 1,
                //    amPm: ['Sa', 'Ch']
            },

            ranges: ranges
        }, cb);
        cb(start, end);

        if (typeof initFinishcallBack === 'function') {
            initFinishcallBack();
        }
    },
    convertToISO: (dateStr) => {
        const [day, month, year, hour, minute] = dateStr.match(/(\d{2})\/(\d{2})\/(\d{4}) (\d{2}):(\d{2})/).slice(1);

        // Tạo đối tượng Date trong múi giờ địa phương
        const localDate = new Date(year, month - 1, day, hour, minute);

        // Định dạng theo kiểu 'yyyy-MM-dd HH:mm'
        const formattedDate = localDate.toISOString().slice(0, 16).replace('T', ' ');

        return formattedDate;
    },
    getDateFileName: (fileName) => {
        var date = new Date();
        // Lấy các thành phần của ngày và giờ
        var year = date.getFullYear();
        var month = ('0' + (date.getMonth() + 1)).slice(-2); // Thêm 0 vào trước nếu cần và lấy 2 chữ số cuối
        var day = ('0' + date.getDate()).slice(-2);
        var hours = ('0' + date.getHours()).slice(-2);
        var minutes = ('0' + date.getMinutes()).slice(-2);
        var seconds = ('0' + date.getSeconds()).slice(-2);
        if (fileName)
            fileName = fileName + '__';
        // Ghép lại thành chuỗi với định dạng yyyyMMdd_HHmmss
        return `${fileName}${year}${month}${day}_${hours}${minutes}${seconds}`;
    }
};
const commonExtension = {
    parseJwt: (token) => {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    },
    refreshToken: async (username, userId, refreshToken) => {
        return new Promise(async (resolve, reject) => {
            try {
                const token = localStorage.getItem('e_atoken');
                const refresh_token = localStorage.getItem('e_rtoken');

                if (token !== null && refresh_token !== null) {
                    const decoded_token = commonFunction.extension.parseJwt(token);
                    const current_time = Math.floor(Date.now() / 1000);

                    if (decoded_token.exp < current_time) {
                        await apiHelper.post(
                            'auth/refresh',
                            {
                                username: username,
                                refresh_token: refreshToken,
                                user_id: userId
                            },
                            async function (response) {
                                if (response.access_token) {
                                    localStorage.setItem('e_atoken', response.access_token);
                                    localStorage.setItem('e_rtoken', response.refresh_token);

                                    var list_menu = response.list_menu;
                                    var list_factory = response.list_factory;
                                    var list_full_location = response.list_full_location;
                                    var list_user_function = response.list_user_function;
                                    await commonFunction.extension.saveAuthorizeToIndexedDB(
                                        'AuthorizeDB',
                                        list_menu,
                                        list_factory,
                                        list_full_location,
                                        list_user_function
                                    );
                                    resolve();
                                } else {
                                    localStorage.removeItem('e_atoken');
                                    localStorage.removeItem('e_rtoken');
                                    window.location.href = '/Auth';
                                    reject('Access token not found in response');
                                }
                            },
                            function (error) {
                                console.error('Exception Error:', error);
                                window.location.href = '/Auth';
                                reject(error);
                            },
                            false
                        );
                    } else {
                        resolve(); // resolve if token is not expired
                    }
                } else {
                    resolve(); // resolve if no token to refresh
                }
            } catch (error) {
                console.log(error);
                reject(error);
            }
        });
    },
    checkHideShowComponent: (apiUrl, componentId) => {
        var list_api_url = [];
        commonFunction.extension.getListApiUrl()
            .then(list => {
                list_api_url = list;
                const hasAccess = list_api_url.some(function (func) {
                    return func.trim().toLowerCase() === apiUrl.trim().toLowerCase();
                });
                if (hasAccess) {
                    $(`#${componentId}`).show();
                } else {
                    $(`#${componentId}`).hide();
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    },
    checkFunctionComponent: async (apiUrl, componentKey) => {
        var list_api_url = await commonFunction.extension.getListApiUrl();
        //console.log("list_api_url\n", list_api_url);
        const hasAccess = list_api_url.some(function (func) {
            return func.trim().toLowerCase() === apiUrl.trim().toLowerCase();
        });

        $(`${componentKey}`).hide();
        if (hasAccess)
            $(`${componentKey}`).show();
    },
    exportBlob: (data, fileNamePrefix) => {
        const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        const currentTime = commonDateTime.getCurrentDateTimeFormatFileName();
        a.download = `${fileNamePrefix}_${currentTime}.xlsx`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
    },
    getListApiUrl: (isReturnLocation = false) => {
        return new Promise((resolve, reject) => {
            const dbName = 'AuthorizeDB';
            const request = indexedDB.open(dbName);

            request.onupgradeneeded = function (event) {
                const db = event.target.result;
                if (!db.objectStoreNames.contains('list_user_function')) {
                    db.createObjectStore('list_user_function', { keyPath: 'id' });
                }
            };

            request.onsuccess = function (event) {
                const db = event.target.result;
                const transaction = db.transaction(['list_user_function'], 'readonly');
                const store = transaction.objectStore('list_user_function');
                const getAllRequest = store.getAll();

                getAllRequest.onsuccess = function () {
                    const list_api_url = getAllRequest.result.map(x => x.api_url);
                    if (isReturnLocation == false) {
                        resolve(list_api_url);
                    } else {
                        resolve(getAllRequest.result);
                    }
                };

                getAllRequest.onerror = function (event) {
                    console.error('Failed to load list_user_function from IndexedDB:', event.target.error);
                    reject(event.target.error);
                };
            };

            request.onerror = function (event) {
                console.error('IndexedDB error:', event.target.error);
                reject(event.target.error);
            };
        });
    },
    /*--------- Query Param -------------------------------*/
    getQueryParam: (param) => {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    },
    getQueryParams: (params) => {
        const queryParams = {};
        params.forEach(param => {
            queryParams[param] = commonFunction.extension.getQueryParam(param);
        });
        return queryParams;
    },
    getListSelectedLocation: async (cbxWorkShopId, cbxTeamId) => {
        var list_location_id = [];
        const workshopVal = $(`#${cbxWorkShopId}`).val();
        const teamVal = $(`#${cbxTeamId}`).val();

        if (!workshopVal || workshopVal == 0) {
            if (!teamVal || teamVal == 0) {
                $(`#${cbxWorkShopId}`).find('option').each(async function () {
                    await list_location_id.push(parseInt($(this).val()));
                });
                $(`#${cbxTeamId}`).find('option').each(async function () {
                    await list_location_id.push(parseInt($(this).val()));
                });
            } else {
                await list_location_id.push(parseInt(teamVal));
            }
        } else {
            await list_location_id.push(parseInt(workshopVal));
            if (!teamVal || teamVal == 0) {
                $(`#${cbxTeamId}`).find('option').each(async function () {
                    await list_location_id.push(parseInt($(this).val()));
                });
            } else {
                await list_location_id.push(parseInt(teamVal));
            }
        }
        return list_location_id;
    },
    redirectToMachineMaster: (params) => {
        if (!params.factoryId === null) {
            params.factoryId = $.cookie('chosenFactoryId')
        }
        const baseUrl = `${window.location.origin}/MachineMaster`;
        const queryString = Object.keys(params)
            .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
            .join('&');
        const url = `${baseUrl}?${queryString}`;
        return url;
    },

    saveAuthorizeToIndexedDB: (dbName, listMenu, listFactory, listFullLocation, listUserFunction) => {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);

            request.onsuccess = function (event) {
                const db = event.target.result;

                // Kiểm tra sự tồn tại của tất cả các object store
                const requiredStores = ['list_menu', 'list_factory', 'list_full_location', 'list_user_function'];
                const missingStores = requiredStores.filter(store => !db.objectStoreNames.contains(store));

                if (missingStores.length > 0) {
                    console.warn('Missing object stores:', missingStores, '. Deleting and recreating database.');

                    // Đóng cơ sở dữ liệu và xóa nó
                    db.close();
                    const deleteRequest = indexedDB.deleteDatabase(dbName);

                    deleteRequest.onsuccess = () => {
                        console.log('Database deleted successfully.');
                        recreateDatabase(); // Tạo lại cơ sở dữ liệu sau khi xóa
                    };

                    deleteRequest.onerror = (event) => {
                        console.error('Failed to delete database:', event.target.error);
                        reject(event.target.error);
                    };

                    return;
                }

                // Nếu không thiếu object store nào, tiến hành thêm dữ liệu
                handleTransaction(db);
            };

            request.onupgradeneeded = function (event) {
                recreateStores(event.target.result);
            };

            request.onerror = function (event) {
                console.error('Failed to open database:', event.target.error);
                reject(event.target.error);
            };

            const recreateDatabase = () => {
                const createRequest = indexedDB.open(dbName);

                createRequest.onupgradeneeded = function (event) {
                    const db = event.target.result;
                    recreateStores(db);
                };

                createRequest.onsuccess = function (event) {
                    const db = event.target.result;
                    handleTransaction(db);
                };

                createRequest.onerror = function (event) {
                    console.error('Failed to recreate database:', event.target.error);
                    reject(event.target.error);
                };
            };

            const recreateStores = (db) => {
                const stores = [
                    { name: 'list_menu', keyPath: 'id', autoIncrement: true },
                    { name: 'list_factory', keyPath: 'id', autoIncrement: true },
                    { name: 'list_full_location', keyPath: 'id', autoIncrement: true },
                    { name: 'list_user_function', keyPath: 'id', autoIncrement: true }
                ];

                stores.forEach(store => {
                    if (!db.objectStoreNames.contains(store.name)) {
                        db.createObjectStore(store.name, { keyPath: store.keyPath, autoIncrement: store.autoIncrement });
                        console.log(`Object store "${store.name}" created.`);
                    }
                });
            };

            const handleTransaction = (dbInstance) => {
                const transaction = dbInstance.transaction(
                    ['list_menu', 'list_factory', 'list_full_location', 'list_user_function'],
                    'readwrite'
                );

                const clearAndInsertData = (storeName, data) => {
                    const store = transaction.objectStore(storeName);

                    store.clear().onsuccess = () => {
                        console.log(`${storeName} cleared.`);
                        data.forEach(item => store.add(item));
                    };

                    store.clear().onerror = (event) => {
                        console.error(`Failed to clear ${storeName}:`, event.target.error);
                        reject(event.target.error);
                    };
                };

                clearAndInsertData('list_menu', listMenu);
                clearAndInsertData('list_factory', listFactory);
                clearAndInsertData('list_full_location', listFullLocation);
                clearAndInsertData('list_user_function', listUserFunction);

                transaction.oncomplete = () => {
                    console.log('All data inserted successfully.');
                    resolve();
                };

                transaction.onerror = (event) => {
                    console.error('Transaction failed:', event.target.error);
                    reject(event.target.error);
                };
            };
        });
    },
    getDictionaryFromIndexedDB: () => {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open("TranslationDB");

            request.onsuccess = function (event) {
                const db = event.target.result;

                // Kiểm tra nếu objectStore "translations" không tồn tại
                if (!db.objectStoreNames.contains("translations")) {
                    console.warn("Object store 'translations' does not exist. Skipping dictionary retrieval.");
                    resolve({}); // Trả về dictionary rỗng
                    return;
                }

                const transaction = db.transaction(["translations"], "readonly");
                const objectStore = transaction.objectStore("translations");
                const getAllRequest = objectStore.getAll();

                getAllRequest.onsuccess = function () {
                    const dictionary = {};
                    getAllRequest.result.forEach(item => {
                        dictionary[item.key] = item.value;
                    });
                    resolve(dictionary);
                };

                getAllRequest.onerror = function () {
                    reject("Error retrieving dictionary from IndexedDB");
                };
            };

            request.onerror = function () {
                reject("Error opening IndexedDB");
            };
        });
    },
    getTranslationByKey: (key) => {
        return new Promise((resolve, reject) => {
            commonFunction.extension.getDictionaryFromIndexedDB().then(dictionary => {
                const language_code = localStorage.getItem('currentLanguage');
                let translation = language_code !== 'vn' ? dictionary[key] : key;
                if (translation === undefined) {
                    translation = key;
                }
                resolve(translation);
            }).catch(error => {
                reject('Failed to fetch dictionary: ' + error);
            });
        });
    },
    getDataFromIndexedDB: async (dbName, storeName) => {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(dbName);
            //request.onupgradeneeded = function (event) {
            //    const db = event.target.result;
            //    if (!db.objectStoreNames.contains(storeName)) {
            //        db.createObjectStore(storeName, { keyPath: "id" });
            //    }
            //};
            request.onsuccess = function (event) {
                const db = event.target.result;
                const transaction = db.transaction([storeName], 'readonly');
                const objectStore = transaction.objectStore(storeName);
                const allData = objectStore.getAll();
                allData.onsuccess = function () {
                    resolve(allData.result);
                };
                allData.onerror = function (event) {
                    reject(event.target.error);
                };
            };
            request.onerror = function (event) {
                reject(event.target.error);
            };
        });
    },
    /*---------------------------------------------------------------------------------------*/
    runSequentially: async (firstAction, callBack) => {
        if (typeof firstAction === 'function') {
            await firstAction();
        }

        if (typeof callBack === 'function') {
            callBack();
        }
    },
    /*---------------------------------------------------------------------------------------*/
    populateLanguageDropdown: async () => {
        // Retrieve listLanguage from localStorage
        const listLanguage = JSON.parse(localStorage.getItem('listLanguage'));
        // Check if listLanguage exists and has items
        if (listLanguage && listLanguage.length > 0) {
            var par = document.getElementById('drop-language');
            par.innerHTML = ''; // Clear any existing items in the dropdown

            // Populate the dropdown with the languages
            listLanguage.forEach(lang => {
                var div = document.createElement('div');
                div.className = "menu-item px-3";
                div.innerHTML = `<a class="menu-link px-3" href="#" onclick="onchangelang('${lang.language_code}')">
                                   <span class="fi fi-${lang.flag}"></span>
                                   &nbsp;${lang.name}
                                </a>`;
                par.appendChild(div);
            });
        } else {
            console.warn('No languages found in localStorage');
        }
    }
}
const commonDataTable = {
    initializeDataTable: (tableId, columns, data, ordering = true, autoWith = true, filter = true, paging = true, fixedHeader = false, scrollY = 0, page = 10) => {
        $(`#${tableId}`).DataTable().clear().destroy();

        var tableSetting = {
            "destroy": true,
            "lengthChange": true,
            "language": {
                "info": "Show _START_ - _END_ ( Total: _TOTAL_ )",
                "infoEmpty": "Showing 0 - 0 ",
                "infoFiltered": "( Total: _MAX_ )",
                "lengthMenu": "Show _MENU_ ",
                "loadingRecords": "Loading ... ",
                "paginate": {
                    'previous': '<',
                    'next': '>',
                    'last': '>>',
                    'first': '<<'
                },
            },
            "pageLength": page,
            "paging": paging,
            "searching": filter,
            "ordering": ordering,
            "autoWidth": autoWith,
            "fixedHeader": fixedHeader,
            "columns": columns,
            "scrollY": scrollY > 0 ? `${scrollY}px` : ``,
            "scrollCollapse": true,
            "info": true,
        };
        var table = $(`#${tableId}`).DataTable(tableSetting);
        table.clear().rows.add(data).draw();
    },
    genOperationButtonTable: (data, list_user_function, onClickEdit, onClickDelete, controller, isCheckPermission = true, isCheckIdMachine = false, idMachine = 0) => {
        var list_api_url = list_user_function.map(x => x.api_url);
        if (isCheckPermission === true) {
            if (data != null && data > 0 && list_user_function != null) {
                const hasAccessDelete = list_api_url.some(function (func) {
                    return func.trim().toLowerCase() == `/${controller}/delete`.toLowerCase();
                });
                let hasAccessUpdate = false;
                if (isCheckIdMachine && idMachine > 0) {
                    hasAccessUpdate = list_user_function.some(function (func) {
                        return func.api_url.trim().toLowerCase() === `/${controller}/update`.toLowerCase() && func.machine_ids.split(',').map(Number).includes(idMachine);
                    });
                } else {
                    hasAccessUpdate = list_api_url.some(function (func) {
                        return func.trim().toLowerCase() == `/${controller}/update`.toLowerCase();
                    });
                }

                const hasAccessViewDetail = list_api_url.some(function (func) {
                    return func.trim().toLowerCase() == `/${controller}/selectbyid`.toLowerCase();
                });
                var button = ``;
                if (hasAccessUpdate) {
                    button += `<i class="ki-duotone ki-pencil text-primary btn-icon" style="font-size:20px" onclick="${onClickEdit}(${data})">
                            <span class="path1"></span>
                            <span class="path2"></span>
                        </i>`;
                } else {
                    if (hasAccessViewDetail) {
                        button += `<i class="ki-duotone ki-eye text-primary btn-icon" style="font-size:20px" onclick="${onClickEdit}(${data})">
                         <span class="path1"></span>
                         <span class="path2"></span>
                         <span class="path3"></span>
                        </i>`;
                    }
                }
                if (hasAccessDelete) {
                    button += `<i class="ki-duotone ki-trash text-danger btn-icon" style="font-size:20px"  onclick="${onClickDelete}(${data})">
                             <span class="path1"></span>
                             <span class="path2"></span>
                             <span class="path3"></span>
                             <span class="path4"></span>
                             <span class="path5"></span>
                            </i>`;
                }
                button += '';
                return button;
            }
            else {
                return ``;
            }
        } else {
            var button = ``;
            button += `<div class='btn-group'><button class="btn btn-info btn-sm" onclick="${onClickEdit}(${data})">
                                        <i class="fa fa-pencil-square-o text-white" aria-hidden="true"></i>
                                        </button>`;
            button +=
                `<button class="btn btn-danger btn-sm" onclick="${onClickDelete}(${data})">
                                                <i class="fa fa-trash-o text-white" aria-hidden="true"></i>
                                            </button></div>`;
            return button;
        }
    },
    genOperationButtonTableWithLocation: (data, list_api_url, onClickEdit, onClickDelete, controller, location_id) => {
        if (data != null && data > 0 && list_api_url != null) {
            const hasAccessDelete = list_api_url.some(function (func) {
                return func.api_url.trim().toLowerCase() === `/${controller}/delete`.toLowerCase() && func.list_location_id.includes(location_id);
            });
            const hasAccessUpdate = list_api_url.some(function (func) {
                return func.api_url.trim().toLowerCase() === `/${controller}/update`.toLowerCase() && func.list_location_id.includes(location_id);
            });
            const hasAccessViewDetail = list_api_url.some(function (func) {
                return func.api_url.trim().toLowerCase() === `/${controller}/selectbyid`.toLowerCase() && func.list_location_id.includes(location_id);
            });
            var button = ``;
            if (hasAccessUpdate) {
                button += `<div class='btn-group'><button class="btn btn-info btn-sm" onclick="${onClickEdit}(${data})">
                                    <i class="fa fa-pencil-square-o text-white" aria-hidden="true"></i>
                                    </button>`;
            } else {
                if (hasAccessViewDetail) {
                    button += `<div class='btn-group'><button class="btn btn-info btn-sm" onclick="${onClickEdit}(${data})">
                                        <i class="fa fa-eye text-white" aria-hidden="true"></i>
                                        </button>`;
                }
            }
            if (hasAccessDelete) {
                button +=
                    `<button class="btn btn-danger btn-sm" onclick="${onClickDelete}(${data})">
                                            <i class="fa fa-trash-o text-white" aria-hidden="true"></i>
                                        </button></div>`;
            }
            return button;
        }
        else {
            return ``;
        }
    },
    genCellActionConfig_old: (config, dataRow) => {
        var html = ``;
        html = `<div class='btn-group'>`;
        if (config) {
            if (config.buttons?.length > 0) { // nếu có button
                var checkPerm = (config.list_url_permission?.length > 0);
                var hasButton = false;

                // Mỗi 1 button config
                config.buttons.forEach((btn, idx, arr) => {
                    if (checkPerm) {
                        hasButton = config.list_url_permission.some(function (func) {
                            return func.trim().toLowerCase() === `/${btn.api}`.toLowerCase();
                        });
                    }
                    if (hasButton) {
                        html += `<button class="${btn.button_class}" onclick="${btn.callback}('${JSON.stringify(dataRow).replace(/\"/g, '`')}')" title="${btn.tooltip}">
                                        <i class="${btn.icon_class}" aria-hidden="true"></i>
                                 </button>`;
                    }
                });
            }
        }
        // ---------------
        html += `</ div>`;

        return html;
    },
    genCellActionConfigWithParam: (config, dataRow) => {
        var html = `<div class="dropdown text-dark">
                         <button class="btn btn-sm btn-light btn-flex btn-center btn-active-light-primary text-dark dropdown-toggle"
                                 type="button"
                                 id="dropdownMenuButton"
                                 data-bs-toggle="dropdown"
                                 aria-expanded="false"
                                 data-i18n="Chức năng">
                             Chức năng
                         </button>
                         <ul class="dropdown-menu dropdown-menu-end w-125px" aria-labelledby="dropdownMenuButton">
                    `;
        if (config) {
            if (config.buttons?.length > 0) { // nếu có button
                var checkPerm = (config.list_url_permission?.length > 0);
                var hasButton = false;

                // Mỗi 1 button config
                config.buttons.forEach((btn, idx, arr) => {
                    if (checkPerm) {
                        hasButton = config.list_url_permission.some(function (func) {
                            return func.trim().toLowerCase() === `/${btn.api}`.toLowerCase();
                        });
                    }
                    if (hasButton) {
                        html += `<li>
                                     <p class="dropdown-item m-0"  onclick="${btn.callback}('${JSON.stringify(dataRow).replace(/\"/g, '`')}')"
                                      data-i18n="${btn.tooltip}">
                                     ${btn.tooltip}</p>
                                 </li>`;
                    }
                });
            }
        }
        html += ` </ul></div>`;

        return html;
    },
    genCellActionConfig: (config, dataRow) => {
        var html = `<div class="dropdown text-dark">
                         <button class="btn btn-sm btn-light btn-flex btn-center btn-active-light-primary text-dark dropdown-toggle"
                                 type="button"
                                 id="dropdownMenuButton"
                                 data-bs-toggle="dropdown"
                                 aria-expanded="false"
                                 data-i18n="Chức năng">
                             Chức năng
                         </button>
                         <ul class="dropdown-menu dropdown-menu-end w-125px" aria-labelledby="dropdownMenuButton">
                    `;
        if (config) {
            if (config.buttons?.length > 0) { // nếu có button
                var checkPerm = (config.list_url_permission?.length > 0);
                var hasButton = false;

                // Mỗi 1 button config
                config.buttons.forEach((btn, idx, arr) => {
                    if (checkPerm) {
                        hasButton = config.list_url_permission.some(function (func) {
                            return func.trim().toLowerCase() === `/${btn.api}`.toLowerCase();
                        });
                    }
                    if (hasButton) {
                        html += `<li>
                                     <p class="dropdown-item m-0" onclick="${btn.callback}(${dataRow})"
                                      data-i18n="${btn.tooltip}">
                                     ${btn.tooltip}</p>
                                 </li>`;
                    }
                });
            }
        }
        html += ` </ul></div>`;

        return html;
    },

    exportDataTableToExcel: (tableID, filename = 'datatable') => {
        var table = $(`#${tableID}`).DataTable();
        var wb = XLSX.utils.book_new();
        var ws_data = [];

        // Lấy tiêu đề của các cột hiển thị, bỏ qua cột cuối cùng
        var headers = [];
        $(`#${tableID} thead th`).each(function (index) {
            if (index < table.columns().header().length - 1) {
                headers.push($(this).text());
            }
        });
        ws_data.push(headers);

        // Lấy dữ liệu của các hàng từ DataTable
        table.rows().every(function () {
            var row_array = [];
            var row_index = this.index();

            // Chỉ lấy dữ liệu của các cột hiển thị, bỏ qua cột cuối cùng
            table.columns().every(function () {
                var column_index = this.index();
                if (column_index < table.columns().header().length - 1) {
                    var cell_data = table.cell({ row: row_index, column: column_index }).data();
                    row_array.push(cell_data);
                }
            });

            ws_data.push(row_array);
        });

        // Tạo worksheet
        var ws = XLSX.utils.aoa_to_sheet(ws_data);
        XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');

        const currentTime = commonDateTime.getCurrentDateTimeFormatFileName();
        filename = `${filename}_${currentTime}.xlsx`;
        // Xuất ra file Excel
        XLSX.writeFile(wb, `${filename}.xlsx`);
    },
    addButtonExportExcel: (tableID, exportFuncName = 'exportExcel()', title = "Xuất file") => {
        if (exportFuncName === 'exportExcel') exportFuncName = 'exportExcel()';

        var rowcount = $(`#${tableID}`).DataTable()?.data()?.length ?? 0;
        if (rowcount > 0) {
            $(`#${tableID}_filter`).append(`<button class="btn btn-sm btn-success btn-excel ms-2" onclick="${exportFuncName}" id="btnAddMachine" title="${title}">
										<i class="bi bi-file-earmark-excel text-white" aria-hidden="true"></i>
									</button>`);
        }
        else {
            $(`#${tableID}`).DataTable().clear().destroy();
        }
    },
    addButtonAddDataRow: (tableID, addDataRowFuncName = 'addDataRow()', title = '') => {
        if (addDataRowFuncName === 'addDataRow') addDataRowFuncName = 'addDataRow()';
        var rowcount = $(`#${tableID}`).DataTable()?.data()?.length ?? 0;
        if (rowcount > 0) {
            $(`#${tableID}_filter`).append(`<button class="btn btn-sm btn-success ms-2" onclick="${addDataRowFuncName}" id="btnAddDataRow_${tableID}" title="${title}">
								                <i class="fa fa-plus-circle text-white" aria-hidden="true"></i>
							                </button>`);
        }
        else {
            $(`#${tableID}`).DataTable().clear().destroy();
        }
    },
    addButtonDynamic: (tableID, clickName = 'viewModal', iconName = 'fa fa-plus-circle') => {
        $(`#${tableID}_filter`).append(`<button class="btn btn-success ms-1" onclick="${clickName}()">
										<i class="${iconName} text-white" aria-hidden="true"></i>
									</button>`);
    },
    resizeAbleColumn: () => {
        $('table th').resizable({
            handles: 'e',
            stop: function (e, ui) {
                $(this).width(ui.size.width);
            }
        });
    }
}
const commonModal = {
    openModal: function (modalId, loadingCallBack) {
        $(`#${modalId}`).modal("show");
        if (loadingCallBack != null && typeof loadingCallBack === 'function') {
            loadingCallBack();
        }
    },
    loadComboBox: async function (comboBoxId, url, data, defaultValue = 0, isAllOption = false, callback = null, method = 'GET') {
        async function populateComboBox(comboBoxId, response, defaultValue, isAllOption, callback) {
            var comboBox = document.getElementById(comboBoxId);
            comboBox.innerHTML = '';

            if (isAllOption === true) {
                var option = document.createElement('option');
                option.setAttribute('data-i18n', 'Tất cả');
                try {
                    let translation = await commonExtension.getTranslationByKey('Tất cả');
                    option.text = translation;
                } catch (error) {
                    console.error('Failed to get translation:', error);
                    option.text = 'Tất cả';
                }
                option.value = 0;
                if (defaultValue === 0) {
                    option.selected = true;
                }
                comboBox.add(option);
            }

            let options = [];
            for (let item of response.data) {
                var option = document.createElement('option');
                try {
                    let translation = await commonExtension.getTranslationByKey(item.text);
                    option.setAttribute('data-i18n', item.text);
                    option.text = translation;
                } catch (error) {
                    console.error('Failed to get translation:', error);
                    option.text = item.text;
                }
                option.value = item.value;
                options.push(option);
            }

            options.forEach(function (option) {
                comboBox.add(option);
            });

            // Đặt giá trị mặc định nếu có
            if (options.length > 0) {
                options.forEach(function (option) {
                    if (option.value == defaultValue) {
                        option.selected = true;
                    }
                });
            }

            if (typeof callback === 'function') {
                callback();
            }
        }

        if (method.toUpperCase() === 'POST') {
            await apiHelper.post(url, data,
                async function (response) {
                    await populateComboBox(comboBoxId, response, defaultValue, isAllOption, callback);
                },
                function (error) {
                    console.error('Exception Error:', error);
                }
            );
        } else {
            await apiHelper.get(url, data,
                async function (response) {
                    await populateComboBox(comboBoxId, response, defaultValue, isAllOption, callback);
                },
                function (error) {
                    console.error('Exception Error:', error);
                }
            );
        }
    },
    closeModal: function (modalId, loadingCallBack) {
        $(`#${modalId}`).modal("hide");
        //if (typeof callback === 'function') {
        loadingCallBack();
        //    }
    },
    validateInputAndFocus: (modalContainerId) => {
        const container = document.getElementById(modalContainerId);
        const inputs = container.querySelectorAll('input[required], textarea[required]');

        for (let input of inputs) {
            if (input.value.trim() === '') {
                input.focus();
                return false;
            }
        }
        return true;
    },
    loadLocationComboBox: async (comboBoxId, defaultValue, isAllOption, callback, locationLevelId = 0, isLoadUnkown = true, locationParentId = 0) => {
        async function populateComboBox(comboBoxId, response, defaultValue, isAllOption) {
            const comboBox = document.getElementById(comboBoxId);
            comboBox.innerHTML = '';

            if (isAllOption === true) {
                const option = document.createElement('option');
                option.setAttribute('data-i18n', 'Tất cả');
                try {
                    const translation = await commonExtension.getTranslationByKey('Tất cả');
                    option.text = translation;
                } catch (error) {
                    console.error('Failed to get translation:', error);
                    option.text = 'Tất cả';
                }
                option.value = 0;
                if (defaultValue === 0) {
                    option.selected = true;
                }
                comboBox.add(option);
            }

            const options = [];
            for (const item of response) {
                const option = document.createElement('option');
                try {
                    const translation = await commonExtension.getTranslationByKey(item.location_name);
                    option.setAttribute('data-i18n', item.location_name);
                    option.text = translation;
                } catch (error) {
                    console.error('Failed to get translation:', error);
                    option.text = item.location_name; // fallback text
                }
                option.value = item.id;
                options.push(option);
            }

            options.forEach(option => comboBox.add(option));

            if (options.length > 0) {
                options.forEach(option => {
                    if (option.value == defaultValue) {
                        option.selected = true;
                    }
                });
            }
        }

        try {
            return new Promise(async (resolve, reject) => {
                let response = await commonExtension.getDataFromIndexedDB('AuthorizeDB', 'list_full_location');
                response = response.filter(x => x.location_level_id !== 2);
                if (locationLevelId > 0) {
                    response = response.filter(x => x.location_level_id === locationLevelId);
                }
                if (!isLoadUnkown) {
                    response = response.filter(x => x.location_name.trim() !== 'Chưa xác định' && x.location_name.trim() !== 'Unknown');
                } else {
                    response = response.sort((a, b) => {
                        if (a.location_name.trim() === 'Chưa xác định') return -1;
                        if (b.location_name.trim() === 'Chưa xác định') return 1;
                        return 0;
                    });
                }

                if (locationParentId > 0) {
                    response = await response.filter(x => x.location_parent_id == locationParentId);
                }
                await populateComboBox(comboBoxId, response, defaultValue, isAllOption);

                resolve();

                if (typeof callback === 'function') {
                    callback();
                }
            });
        } catch (error) {
            console.error('Failed to load data from IndexedDB:', error);
            return Promise.reject(error);
        }
    }
}
const commonAnimation = {
    showLoading: (loadingId, buttonId) => {
        document.getElementById(loadingId).style.display = 'flex';
        document.getElementById(buttonId).disabled = true;
    },
    hideLoading: (loadingId, buttonId) => {
        document.getElementById(loadingId).style.display = 'none';
        document.getElementById(buttonId).disabled = false;
    },
    showLoadingNewUI: () => {
        document.getElementById("loadingOverlay").style.display = "flex";
        document.getElementById("kt_app_main").classList.add("loading-active");
    },
    hideLoadingNewUI: () => {
        document.getElementById("loadingOverlay").style.display = "none";
        document.getElementById("kt_app_main").classList.remove("loading-active");
    },
    hexToRgbaColor: (hex, alpha) => {
        // Xử lý mã màu hex nếu có dấu # ở đầu
        hex = hex.replace(/^#/, '');

        // Chuyển đổi mã màu hex thành các giá trị đỏ, xanh lá và xanh dương
        let r = parseInt(hex.substring(0, 2), 16);
        let g = parseInt(hex.substring(2, 4), 16);
        let b = parseInt(hex.substring(4, 6), 16);

        // Trả về chuỗi màu rgba với độ trong suốt
        return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    },
    replaceBrokenImages: () => {
        const checkImage = (url, fallback = 'imgs/weldcom_watermark.jpg') => {
            return new Promise((resolve) => {
                const imgTest = new Image();
                imgTest.onload = () => resolve(url);
                imgTest.onerror = () => resolve(fallback);
                imgTest.src = url;
            });
        };
        const images = document.querySelectorAll('img');
        images.forEach(img => {
            checkImage(img.src).then((src) => {
                img.src = src
            });
        });
    },
    toggleButtonState: (buttonId, isLoading) => {
        let btn = document.getElementById(buttonId);
        if (!btn) return; // Kiểm tra nếu nút không tồn tại

        if (isLoading) {
            // Chuyển sang trạng thái "Please wait..."
            btn.disabled = true; // Vô hiệu hóa nút
            btn.querySelector('.indicator-label').style.display = 'none'; // Ẩn "Submit"
            btn.querySelector('.indicator-progress').style.display = 'inline-flex'; // Hiển thị "Please wait..."
        } else {
            // Chuyển lại trạng thái "Submit"
            btn.disabled = false; // Kích hoạt nút
            btn.querySelector('.indicator-label').style.display = 'inline-flex'; // Hiển thị "Submit"
            btn.querySelector('.indicator-progress').style.display = 'none'; // Ẩn "Please wait..."
        }
    }
}
const commonChart = {
    createCustomPattern: (patternDraw, patternColor, backgroundColor) => {
        let shape = document.createElement('canvas');
        shape.width = 10;
        shape.height = 10;
        let c = shape.getContext('2d');

        // Fill the background color
        c.fillStyle = backgroundColor;
        c.fillRect(0, 0, shape.width, shape.height);

        // Set the pattern color
        c.strokeStyle = patternColor;
        c.fillStyle = patternColor;

        // Draw the specified pattern
        switch (patternDraw) {
            case 'diagonal':
                c.beginPath();
                c.moveTo(2, 0);
                c.lineTo(10, 8);
                c.stroke();
                c.beginPath();
                c.moveTo(0, 8);
                c.lineTo(2, 10);
                c.stroke();
                break;
            case 'vertical':
                c.beginPath();
                c.moveTo(5, 0);
                c.lineTo(5, 10);
                c.stroke();
                break;
            case 'rectangle':
                c.fillRect(2, 2, 6, 6);
                break;
            default:
                break;
        }

        return c.createPattern(shape, 'repeat');
    },
    createBadge: (label, rate, patternDraw, patternColor, backgroundColor) => {
        let pattern = commonChart.createCustomPattern(patternDraw, patternColor, backgroundColor);
        let badge = document.createElement('div');
        badge.className = 'badge';
        badge.style.background = `url(${pattern})`;
        badge.textContent = `${label}: ${rate}`;
        badge.style.cursor = 'pointer';
        badge.dataset.chartLabel = label;
        badge.dataset.visible = 'true';
        return badge;
    },
    machineTimeChart: async (machineName, chartId, statusData, powerData, timeUnit, displayFormat, imgUrl) => {
        var ctx = document.getElementById(chartId).getContext('2d');
        var statusDatasets = [];
        var statusLabels = [];

        // Tạo dataset cho trạng thái
        if (statusData != null && statusData.length > 0) {
            const translatedStatusNames = await Promise.all(statusData.map(async (status) => {
                status.status_name = await commonExtension.getTranslationByKey(status.status_name);
                return await commonExtension.getTranslationByKey(status.status_name);
            }));
            statusDatasets = statusData.map(status => ({
                label: `${status.status_name}: ${status.rate}% - ${status.total_hour}h`,
                data: status.range_time.map(range => ({
                    x: [range.start_time, range.end_time],
                    y: status.status_name
                })),
                backgroundColor: commonChart.createCustomPattern(status.status_pattern_draw, status.status_pattern_color,
                    commonAnimation.hexToRgbaColor(status.status_color, 0.4)),
                borderColor: status.status_color,
                borderWidth: 1,
                barThickness: 20,
                borderSkipped: false,
                stack: 'status',
                type: 'bar',
                zIndex: 1,
            }));
            statusLabels = translatedStatusNames;
        }

        // Tạo dataset cho powerData
        var datasets = [...statusDatasets];
        var powerDataset = {};
        var minPower = 0;
        var maxPower = 0;
        //if (powerData != null && powerData.length > 0) {
        //    minPower = Math.min(...powerData.map(dataPoint => dataPoint.kwh));
        //    maxPower = Math.max(...powerData.map(dataPoint => dataPoint.kwh));

        //    powerDataset = {
        //        type: 'line',
        //        label: 'Kwh',
        //        data: powerData.map(dataPoint => ({
        //            x: dataPoint.date_point,
        //            y: dataPoint.kwh
        //        })),
        //        backgroundColor: 'rgba(54, 162, 235, 0.2)',
        //        borderColor: 'rgba(54, 162, 235, 1)',
        //        borderWidth: 2,
        //        fill: false,
        //        yAxisID: 'y2',
        //        zIndex: 2,
        //    };
        //    if (powerDataset != null) {
        //        datasets.push(powerDataset);
        //    }
        //}
        const colorText = localStorage.getItem('data-bs-color') ?? 'white';
        // Create chart
        var myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                datasets: datasets,
                // legend set to false if you don't want it
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'y',
                scales: {
                    x: {
                        type: 'time',
                        position: 'bottom',
                        time: {
                            unit: timeUnit,
                            displayFormats: {
                                [timeUnit]: displayFormat
                            }
                        },
                        title: {
                            display: true,
                            text: 'Time',
                            color: colorText // Make the X-axis title white
                        },
                        grid: {
                            display: true
                        },
                        ticks: {
                            color: colorText // Set X-axis label color to white
                        }
                    },
                    y: {
                        type: 'category',
                        labels: statusLabels,
                        position: 'left',
                        grid: {
                            display: true
                        },
                        ticks: {
                            color: colorText // Set Y-axis label color to white
                        }
                    },
                    //y2: {
                    //    type: 'linear',
                    //    position: 'right',
                    //    title: {
                    //        display: true,
                    //        text: 'Kwh',
                    //        color: colorText // Make the Y2-axis title white
                    //    },
                    //    ticks: {
                    //        beginAtZero: true,
                    //        stepSize: 1,
                    //        color: colorText // Set Y2-axis label color to white
                    //    },
                    //    grid: {
                    //        display: false
                    //    },
                    //    min: minPower,
                    //    max: maxPower,
                    //}
                },
                plugins: {
                    legend: {
                        labels: {
                            color: colorText // Set legend text color to white
                        }
                    },
                    tooltip: {
                        titleColor: 'white', // Tooltip title color
                        bodyColor: 'white',  // Tooltip body text color
                        callbacks: {
                            label: function (context) {
                                if (context.dataset.type === 'bar') {
                                    const startTime = commonDateTime.formatDateChart(new Date(context.raw.x[0]));
                                    const endTime = commonDateTime.formatDateChart(new Date(context.raw.x[1]));
                                    return `${startTime} - ${endTime}`;
                                } else {
                                    return context.dataset.label + ': ' + context.raw.y + ' Kwh';
                                }
                            }
                        }
                    }
                }
            },
        });
    },
    exportPNG: (canvasId, fileName) => {
        var canvas = document.getElementById(canvasId);
        const link = document.createElement('a');
        link.href = canvas.toDataURL("image/png");
        link.download = `${fileName}_${commonDateTime.getCurrentDateTimeFormatted()}.png`;
        link.click();
    },
    exportPDF: (canvasId, fileName) => {
        html2canvas(document.getElementById(canvasId), {
            onrendered: function (canvas) {
                var img = canvas.toDataURL(); // image data of canvas
                var doc = new jsPDF();
                doc.addImage(img, 10, 10);
                doc.save(`${fileName}_${commonDateTime.getCurrentDateTimeFormatted()}.pdf`);
            }
        });
    },
    exportExcelData: async (canvasId, sheetData, fileName) => {
        const canvas = document.getElementById(canvasId);

        // Tạo workbook và worksheet
        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet('Chart Data');

        // Lấy ảnh từ canvas
        const chartImage = canvas.toDataURL('image/png');
        const imgData = chartImage.split(',')[1];
        const imgBlob = new Blob([new Uint8Array(atob(imgData).split('').map(char => char.charCodeAt(0)))], { type: 'image/png' });
        const imgBuffer = await imgBlob.arrayBuffer();

        // Thêm ảnh vào workbook
        const imageId = workbook.addImage({
            buffer: imgBuffer,
            extension: 'png',
        });

        const imgWidth = 1200;
        const imgHeight = 400;

        worksheet.addImage(imageId, {
            tl: { col: 0, row: 0 },
            ext: { width: imgWidth, height: imgHeight }
        });

        worksheet.getColumn(1).width = imgWidth / 8;
        worksheet.getRow(1).height = imgHeight;

        worksheet.addRows(sheetData, { header: true });

        workbook.xlsx.writeBuffer().then(buffer => {
            const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `${fileName}_${commonDateTime.getCurrentDateTimeFormatted()}.xlsx`;
            a.click();
            URL.revokeObjectURL(url);
        });
    },
    createPyramidChartApex: (canvasID, listData, type, isLegend = true, onClickCallback) => {
        const filteredData = listData
            .filter(item => item.rate > 0)
            .sort((a, b) => a.rate - b.rate);
        const labels = type === 'group' ? filteredData.map(item => item.machine_group) : filteredData.map(item => item.machine_type);
        const data = filteredData.map(item => item.rate);
        const colors = filteredData.map(item => item.color);

        var options = {
            series: [
                {
                    name: "",
                    data: data,
                }
            ],
            chart: {
                type: 'bar',
                height: 220,
                background: 'transparent', // Đặt nền của biểu đồ là trong suốt,
                events: {
                    dataPointSelection: function (event, chartContext, config) {
                        const label = labels[config.dataPointIndex];
                        onClickCallback(label);
                    }
                }
            },
            plotOptions: {
                bar: {
                    borderRadius: 3,
                    horizontal: true,
                    distributed: true,
                    barHeight: '80%',
                    isFunnel: true, // Tạo dạng chóp
                },
            },
            colors: colors,
            dataLabels: {
                enabled: true,
                formatter: function (val, opt) {
                    return labels[opt.dataPointIndex];
                },
                dropShadow: {
                    enabled: true,
                },
            },
            xaxis: {
                categories: labels,
            },
            tooltip: {
                enabled: true,
                y: {
                    formatter: function (val) {
                        return `${val}%`;
                    }
                }
            },
            legend: {
                show: isLegend,
            },
        };

        var chart = new ApexCharts(document.querySelector(`#${canvasID}`), options);

        chart.render().catch(error => {
            console.error('Error rendering chart:', error);
        });
    },
    getRandomColor: (existingColors) => {
        let color;
        do {
            color = `#${Math.floor(Math.random() * 16777215).toString(16)}`;
        } while (existingColors.includes(color));
        return color;
    },
}
const commonFormSerialize = {
    serializeForm(form_id) {
        var form = document.querySelector(`#${form_id}`);
        const formData = {};  // Đối tượng chứa kết quả

        const inputs = form.querySelectorAll('input, select, textarea'); // Lấy tất cả các input trong form

        inputs.forEach(input => {
            const name = input.name;

            // Bỏ qua các field không có tên (không cần serialize)
            if (!name) return;

            switch (input.type) {
                case 'checkbox':
                    formData[name] = input.checked;
                    break;

                case 'radio':
                    // Chỉ thêm radio nếu nó được chọn
                    if (input.checked) {
                        formData[name] = input.value;
                    }
                    break;

                default:
                    // Xử lý các input khác (text, email, number, textarea, select, etc.)
                    formData[name] = input.value;
                    break;
            }
        });

        return formData;
    }
    ,
    formToObject: (formId) => {
        var formArray = $('#' + formId).serializeArray();
        var object = {};
        $.each(formArray, function () {
            if (object[this.name]) {
                if (!Array.isArray(object[this.name])) {
                    object[this.name] = [object[this.name]];
                }
                object[this.name].push(this.value);
            } else {
                object[this.name] = this.value;
            }
        });
        return object;
    },
    bindObjectToForm: (formId, dataObject) => {
        var form = document.getElementById(formId);
        var elements = form.elements;
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            if (dataObject.hasOwnProperty(element.name)) {
                if (element.type === 'checkbox') {
                    element.checked = (dataObject[element.name] === 'yes' || dataObject[element.name] === true);
                } else {
                    element.value = dataObject[element.name];
                }
            }
        }
    },
    resetForm: (formId) => {
        var form = document.getElementById(formId);
        var elements = form.elements;

        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];

            if (element.type === 'checkbox') {
                element.checked = false;
            } else {
                element.value = null;
            }
        }
    },
}
const commonIsNull = {
    // Function to check if an element is empty or null
    IsNullOrEmpty: (type, element, message) => {
        const value = element.val(); // Get the value of the element

        // Check if the value is null, undefined, or an empty string
        if (!value || value.trim() === '') {
            toastr.warning(message);
            element.focus(); // Set focus on the element that failed validation
            return true; // The element is empty
        }
        return false; // The element is not empty
    },
}
const commonExportChart = {
    exportPDFChart: (keyID = "chart1Container", name = "chart") => {
        // Chọn container của biểu đồ
        const chartContainer = document.getElementById(keyID);

        // Dùng html2canvas để chụp ảnh container
        html2canvas(chartContainer).then(function (canvas) {
            const imgData = canvas.toDataURL("image/png");

            // Xuất ra PDF
            const { jsPDF } = window.jspdf;
            const pdf = new jsPDF("landscape");
            pdf.addImage(imgData, "PNG", 10, 10, 280, 150);
            pdf.save(`${name}.pdf`);
        });
    },
    exportPNGChart: (keyID = "chart1Container", name = "chart") => {
        // Chọn container của biểu đồ
        const chartContainer = document.getElementById(keyID);

        // Dùng html2canvas để chụp ảnh container
        html2canvas(chartContainer).then(function (canvas) {
            const imgData = canvas.toDataURL("image/png");

            // Xuất ra PNG
            const link = document.createElement("a");
            link.href = imgData;
            link.download = `${name}.png`; // Tên file PNG xuất ra
            link.click();
        });
    }
}
const commonLanguage = {
    saveDictionaryToIndexedDB: (language_code) => {
        return new Promise((resolve, reject) => {
            apiHelper.get(`dictionary`, {},
                function (response) {
                    // Lưu dictionary vào IndexedDB
                    commonLanguage.storeDictionary(response.data, language_code).then(() => {
                        console.log("Saved Dictionary to IndexedDB", moment().format('DD-MM-YYYY HH:mm:ss'));
                        resolve();
                    }).catch(error => {
                        console.error("Failed to save dictionary:", error);
                        reject(error);
                    });

                    // Lưu danh sách ngôn ngữ vào localStorage
                    if (response.listLanguage) {
                        localStorage.setItem('listLanguage', JSON.stringify(response.listLanguage));
                        console.log("ListLanguage saved to localStorage", moment().format('DD-MM-YYYY HH:mm:ss'));
                    } else {
                        console.warn('No listLanguage found in the response');
                    }
                },
                function (error) {
                    console.error('Failed to load dictionary:', error);
                    reject(error);
                }, false
            );
        });
    },

    storeDictionary: (data, language_code) => {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open("TranslationDB", 3);

            request.onupgradeneeded = function (event) {
                const db = event.target.result;
                if (!db.objectStoreNames.contains("translations")) {
                    const objectStore = db.createObjectStore("translations", { keyPath: "key" });
                    objectStore.createIndex("value", "value", { unique: false });
                }
            };

            request.onsuccess = function (event) {
                const db = event.target.result;
                const transaction = db.transaction(["translations"], "readwrite");
                const objectStore = transaction.objectStore("translations");

                objectStore.clear().onsuccess = function () {
                    console.log("Cleared old data in translations");
                    const transformedData = {}; // Khai báo biến này trước khi sử dụng

                    data.forEach(item => {
                        const { id, ...rest } = item; // Loại bỏ id
                        const key = item.code;

                        if (key) {
                            transformedData[key] = rest;
                            objectStore.put({ key, value: rest }); // Lưu vào IndexedDB
                        }
                    });


                    transaction.oncomplete = function () {
                        console.log("Dictionary saved successfully to IndexedDB");
                        resolve();
                    };

                    transaction.onerror = function (event) {
                        console.error("Transaction failed:", event.target.error);
                        reject("Error saving dictionary: " + event.target.errorCode);
                    };
                };
            };

            request.onerror = function (event) {
                console.error("Error opening IndexedDB:", event.target.error);
                reject("Error opening IndexedDB: " + event.target.errorCode);
            };
        });
    },

    applyTranslations: (language_code) => {
        // Lấy ngôn ngữ hiện tại từ localStorage
        const selectedLanguage = localStorage.getItem('e_language') || language_code;

        commonFunction.extension.getDictionaryFromIndexedDB().then(dictionary => {
            document.querySelectorAll('[data-i18n]').forEach(element => {
                const key = element.getAttribute('data-i18n');

                if (dictionary[key] && dictionary[key][selectedLanguage]) {
                    element.textContent = dictionary[key][selectedLanguage]; // Gán giá trị dịch
                } else {
                    if (dictionary[key]?.[selectedLanguage] != null)
                        element.textContent = dictionary[key]?.["vn"] || key;
                }
            });

            // Dispatch sự kiện khi ngôn ngữ thay đổi
            const event = new CustomEvent('languageChanged', { detail: { language_code: selectedLanguage } });
            document.dispatchEvent(event);
        }).catch(error => {
            console.error('Failed to fetch dictionary:', error);
        });
    },

};


const commonFunction = {
    languge: commonLanguage,
    extension: commonExtension,
    modal: commonModal,
    animation: commonAnimation,
    datetime: commonDateTime,
    chart: commonChart,
    datatable: commonDataTable,
    formserialize: commonFormSerialize,
    isnull: commonIsNull,
    export: commonExportChart,
};