import api from './api';

const salaryCompositionSystemService = {
    getAll() {
        return api.get('/SalaryCompositionSystems');
    }
};

export default salaryCompositionSystemService;
