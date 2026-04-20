import api from './api';

/**
 * Interface cho dữ liệu Candidate (Ứng viên)
 */
export interface Candidate {
    candidateId?: string;
    name: string;
    phone: string;
    email: string;
    hiringCampaign: string;
    hiringPosition: string;
    hiringRound: string;
    review: string;
    hiringAt: string;
    status: boolean;
    state?: number; // ModelState: 1 - Insert, 2 - Update, 3 - Delete
}

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
    keyword?: string;
    columnFilters?: Array<{
        column: string;
        value: string;
        dataType?: number;
        filterType?: number;
    }>;
}

/**
 * Service xử lý các nghiệp vụ liên quan đến Ứng viên (Candidate)
 * @author: xuán hoán (18/04/2026)
 */
const candidateService = {
    /**
     * Lấy danh sách ứng viên có phân trang và tìm kiếm
     * @param pageable Tham số phân trang (pageIndex, pageSize)
     * @param filterRequest Tham số lọc và tìm kiếm (keyword, columnFilters)
     * @returns Promise chứa danh sách dữ liệu và thông tin phân trang
     */
    getPaginated(pageable: Pageable, filterRequest: FilterRequest) {
        return api.get('/Candidates', {
            params: {
                ...pageable,
                ...filterRequest
            }
        });
    },

    /**
     * Thêm mới một ứng viên
     * @param candidate Đối tượng ứng viên cần thêm
     * @returns Promise kết quả từ server
     */
    add(candidate: Candidate) {
        console.log(candidate);
        return api.post('/Candidates', candidate);
    },

    /**
     * Cập nhật thông tin ứng viên
     * @param id ID của ứng viên cần cập nhật
     * @param candidate Đối tượng ứng viên chứa thông tin mới
     * @returns Promise kết quả từ server
     */
    update(id: string, candidate: Candidate) {
        return api.put(`/Candidates/${id}`, candidate);
    },
    
    /**
     * Có thể thêm các helper method khác ở đây
     */
};

export default candidateService;
