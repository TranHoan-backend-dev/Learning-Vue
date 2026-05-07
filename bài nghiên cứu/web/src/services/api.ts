import axios from 'axios';

/**
 * Cấu hình instance axios chung cho toàn ứng dụng
 * @author: xuan hoan (18/04/2026)
 */
const api = axios.create({
    baseURL: 'http://localhost:5156/v1/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

// Thêm interceptor nếu cần (xử lý lỗi chung, thêm token...)
api.interceptors.response.use(
    (response) => response,
    (error) => {
        console.error('API Error:', error.response || error.message);
        return Promise.reject(error);
    }
);

export default api;
