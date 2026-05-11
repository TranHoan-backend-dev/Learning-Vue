import api from './api';

const organizationService = {
    getAll() {
        return api.get('/Organizations');
    }
};

export default organizationService;
