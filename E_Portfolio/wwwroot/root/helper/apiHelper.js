const apiHelper = (function () {
    async function sendRequest({
        url,
        method = 'GET',
        data = null,
        success,
        error,
        isAddToken = true,
        isFormData = false,
        isBlob = false,
    }) {
        try {
            const token = isAddToken ? await getApiToken(url) : localStorage.getItem("e_atoken");

            $.ajax({
                url: url,
                method: method,
                async: true,
                data: isFormData ? data : (data ? JSON.stringify(data) : null),
                contentType: isFormData ? false : (data ? 'application/json; charset=utf-8' : undefined),
                processData: !isFormData,
                xhrFields: isBlob ? { responseType: 'blob' } : undefined,
                beforeSend: function (xhr) {
                    if (token) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                    }
                    if (isBlob) {
                        xhr.overrideMimeType('application/octet-stream');
                    }
                },
                success: success,
                error: function (xhr) {
                    console.error(`API ${method} Error:`, xhr);
                    if (typeof error === 'function') error(xhr);
                }
            });
        } catch (err) {
            console.error('Token/API setup failed:', err);
            if (typeof error === 'function') error(err);
        }
    }

    return {
        get: async function (url, data, success, error, isAddToken = true) {
            await sendRequest({
                url: `${url}?${$.param(data || {})}`,
                method: 'GET',
                success,
                error,
                isAddToken
            });
        },

        post: async function (url, data, success, error, isAddToken = true, isFormData = false, isBlob = false) {
            await sendRequest({
                url,
                method: 'POST',
                data,
                success,
                error,
                isAddToken,
                isFormData,
                isBlob,
            });
        },

        put: async function (url, data, success, error, isAddToken = true) {
            await sendRequest({
                url,
                method: 'PUT',
                data,
                success,
                error,
                isAddToken
            });
        },

        delete: async function (url, id, success, error, isAddToken = true) {
            await sendRequest({
                url: `${url}?id=${id}`,
                method: 'DELETE',
                success,
                error,
                isAddToken
            });
        },

        getFile: async function (url, data, success, error, isAddToken = true) {
            await sendRequest({
                url: `${url}?${$.param(data || {})}`,
                method: 'GET',
                success,
                error,
                isAddToken,
                isBlob: true
            });
        },

        postFile: async function (url, data, success, error, isAddToken = true) {
            await sendRequest({
                url,
                method: 'POST',
                data,
                success,
                error,
                isAddToken,
                isBlob: true
            });
        }
    };
})();


async function configIfTokenExpired() {
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
                            username: decoded_token.unique_name,
                            refresh_token: refresh_token,
                            user_id: decoded_token.nameid
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
                                window.location.href = '/Login';
                                reject('Access token not found in response');
                            }
                        },
                        function (error) {
                            console.error('Exception Error:', error);
                            window.location.href = '/Login';
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
}
async function getApiToken(api_url) {
    await configIfTokenExpired();

    return new Promise((resolve, reject) => {
        const dbName = 'AuthorizeDB';
        const request = indexedDB.open(dbName);

        request.onupgradeneeded = function (event) {
            const db = event.target.result;

            // Create the list_user_function object store if it doesn't already exist
            if (!db.objectStoreNames.contains('list_user_function')) {
                db.createObjectStore('list_user_function', { keyPath: 'id', autoIncrement: true });
            }
        };

        request.onsuccess = function (event) {
            const db = event.target.result;

            // Check if list_user_function exists in the database before attempting to access it
            if (!db.objectStoreNames.contains('list_user_function')) {
                reject('Object store list_user_function not found in database');
                return;
            }

            const transaction = db.transaction('list_user_function', 'readonly');
            const store = transaction.objectStore('list_user_function');

            // Retrieve all items from list_user_function
            const getAllRequest = store.getAll();
            getAllRequest.onsuccess = function () {
                const allRecords = getAllRequest.result;

                api_url = "/" + api_url;
                api_url = api_url.replace(/\/\//, '/');

                // Find the record matching the api_url
                const record = allRecords.find(item => item.api_url.toLowerCase() === api_url.toLowerCase());
                if (record) {
                    resolve(record.api_token); // Return the api_token if found
                } else {
                    resolve(localStorage.getItem("e_atoken")); // Return the api_token if found
                }
            };

            getAllRequest.onerror = function () {
                reject('Failed to load list_user_function from IndexedDB');
            };
        };

        request.onerror = function () {
            reject('Error with IndexedDB');
        };
    });
}

