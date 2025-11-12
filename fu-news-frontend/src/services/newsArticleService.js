import api from './api';

export const newsArticleService = {
  getActive: async () => {
    const response = await api.get('/newsarticles/active');
    return response.data;
  },

  getAll: async () => {
    const response = await api.get('/newsarticles');
    return response.data;
  },

  getMyArticles: async () => {
    const response = await api.get('/newsarticles/my-articles');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/newsarticles/${id}`);
    return response.data;
  },

  search: async (keyword) => {
    const response = await api.get('/newsarticles/search', {
      params: { keyword },
    });
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/newsarticles', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/newsarticles/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/newsarticles/${id}`);
    return response.data;
  },
};
