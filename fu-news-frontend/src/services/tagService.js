import api from './api';

export const tagService = {
  getAll: async () => {
    const response = await api.get('/tags');
    return response.data;
  },
};
