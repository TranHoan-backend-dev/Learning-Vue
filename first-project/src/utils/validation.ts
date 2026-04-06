/**
 * Xác minh sdt. Sdt phải đúng format và không trùng
 * @param {*} phone Sdt form gửi về
 */
export const validatePhone = (phone: string) => {
    const regex = /^[0-9]{10}$/
    if (!regex.test(phone)) {
        throw new Error("Sdt không đúng định dạng")
    }
}

/**
 * Xác minh dữ liệu đầu vào của trường fullName
 * @param {*} fullName Dữ liệu form gửi về
 */
export const validateFullName = (fullName: string) => {
    const regex = /^[\p{L} ]+$/u;
    if (!regex.test(fullName)) {
        throw new Error("Họ tên không được chứa chữ số hoặc ký tự đặc biệt");
    }
}

export const validateEmail = (email: string) => {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!regex.test(email)) {
        throw new Error("Email không đúng định dạng");
    }
}