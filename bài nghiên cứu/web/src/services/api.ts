import axios from 'axios';

/**
 * Cấu hình instance axios chung cho toàn ứng dụng
 * @author: xuan hoan (18/04/2026)
 */
const isHttps = typeof window !== 'undefined' && window.location.protocol === 'https:';
const BASE_URL = isHttps 
    ? 'https://localhost:8080/api/v1' 
    : 'http://localhost:8000/api/v1';

const api = axios.create({
    baseURL: BASE_URL,
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
