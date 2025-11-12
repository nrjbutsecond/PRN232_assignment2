import api from './api';

export const accountService = {
  getAll: async () => {
    const response = await api.get('/accounts');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/accounts/${id}`);
    return response.data;
  },

  search: async (searchTerm) => {
    const response = await api.get('/accounts/search', {
      params: { searchTerm },
    });
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/accounts', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/accounts/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/accounts/${id}`);
    return response.data;
  },
};
