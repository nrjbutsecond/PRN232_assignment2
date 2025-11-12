import api from './api';

export const categoryService = {
  getActive: async () => {
    const response = await api.get('/category/active');
    return response.data;
  },

  getAll: async () => {
    const response = await api.get('/category');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/category/${id}`);
    return response.data;
  },

  getSubCategories: async (parentId) => {
    const response = await api.get(`/category/${parentId}/subcategories`);
    return response.data;
  },

  search: async (keyword) => {
    const response = await api.get('/category/search', {
      params: { keyword },
    });
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/category', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/category/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/category/${id}`);
    return response.data;
  },

  toggleStatus: async (id) => {
    const response = await api.patch(`/category/${id}/toggle-status`);
    return response.data;
  },
};
