import api from './api';

/**
 * Interface cho tham số phân trang
 */
export interface Pageable {
    pageIndex: number;
    pageSize: number;
}

/**
 * Interface cho tham số lọc
 */
export interface FilterRequest {
    Keyword?: string;
    ColumnFilters?: Array<{
        Column: string;
        Value: string;
        DataType?: number;
        FilterType?: number;
    }>;
}

/**
 * Service xử lý các nghiệp vụ liên quan đến Thành phần lương (Salary Composition)
 */
const salaryCompositionService = {
    /**
     * Lấy danh sách thành phần lương có phân trang và lọc
     * @param pageable Tham số phân trang (pageIndex, pageSize)
     * @param filterRequest Tham số lọc và tìm kiếm
     */
    getFilter(pageable: Pageable, filterRequest: FilterRequest) {
        return api.post('/SalaryCompositions/filter', filterRequest, {
            params: pageable
        });
    },

    /**
     * Thêm mới một thành phần lương
     */
    create(data: any) {
        return api.post('/SalaryCompositions', data);
    },

    /**
     * Cập nhật thành phần lương
     */
    update(id: string, data: any) {
        return api.put(`/SalaryCompositions/${id}`, data);
    },

    /**
     * Xóa thành phần lương
     */
    delete(id: string) {
        return api.delete(`/SalaryCompositions`, { data: [id] });
    },

    /**
     * Xóa nhiều thành phần lương
     */
    deleteMany(ids: string[]) {
        return api.delete('/SalaryCompositions', { data: ids });
    }
};

export default salaryCompositionService;
