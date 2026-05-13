import api from './api';

/**
 * Service xử lý các nghiệp vụ liên quan đến Cấu hình Grid (pa_grid_config)
 * @author: xuan hoan (12/05/2026)
 */
const gridConfigService = {
    /**
     * Lấy danh sách cột của một Grid theo gridId
     * @param gridId Định danh của grid (vd: SalaryComponentGrid)
     */
    getByGridId(gridId: string) {
        return api.get(`/GridConfigs/${gridId}`);
    },

    /**
     * Cập nhật danh sách cấu hình cột
     * @param configs Danh sách cấu hình cột mới
     */
    updateRange(configs: any[]) {
        return api.put('/GridConfigs', configs);
    }
};

export default gridConfigService;
