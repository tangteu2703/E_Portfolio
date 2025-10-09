//const apiBase = "http://192.168.22.124:81/api";
const apiBase = "https://localhost:44334/api";

$(document).ready(function () {

    commonIndexDB.setupUser();

    pingAPI(); // Gọi ngay 1 lần khi load

    // Ping lại 2 tiếng = 2 x 60 phút × 60 giây × 1000 ms
    setInterval(pingAPI, 7200000);
});

const pingAPI = () => {
    apiHelper.get(`/Masters/Version-API`, {},
        function (res) {
            console.log("Ping Version: ", res);
        },
        function (err) {
            console.error("Ping error:", err);
        });
}

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
            url = apiBase + url;

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

    async function sendRequestFile(options) {
        const {
            url,
            method = 'POST',
            data,
            success = () => { },
            error = () => { },
            isAddToken = true,
            isBlob = false
        } = options;

        const headers = {};

        if (isAddToken) {
            const token = isAddToken ? await getApiToken(url) : localStorage.getItem("e_atoken");
            if (token) {
                headers['Authorization'] = 'Bearer ' + token;
            }
        }

        $.ajax({
            url: apiBase + url,
            method: method,
            data: data, // data phải là FormData
            headers: headers,
            contentType: false,         // ❗ để browser tự set multipart/form-data
            processData: false,         // ❗ không xử lý data
            enctype: 'multipart/form-data',
            xhrFields: isBlob ? { responseType: 'blob' } : {},
            success: success,
            error: error
        });
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

        postFile: async function (url, data, success, error, isAddToken = true, isBlob = false) {
            await sendRequestFile({
                url,
                method: 'POST',
                data,
                success,
                error,
                isAddToken,
                isBlob: isBlob
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
    return localStorage.getItem('e_atoken'); // ✅ TRẢ VỀ TOKEN THỰC SỰ
}

