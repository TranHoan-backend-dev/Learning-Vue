import api from './api';

const salaryCompositionSystemService = {
    /**
     * Lấy danh sách danh mục hệ thống có phân trang và lọc
     */
    getAll(pageable: any, filterRequest: any) {
        return api.get('/SalaryCompositionSystems', {
            params: {
                ...pageable,
                ...filterRequest
            }
        });
    }
};

export default salaryCompositionSystemService;
