import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { newsArticleService } from '../services/newsArticleService';
import { categoryService } from '../services/categoryService';
import { useAuth } from '../contexts/AuthContext';

export default function NewsArticles() {
  const { user, isAdmin, isStaff } = useAuth();
  const navigate = useNavigate();
  const [articles, setArticles] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [viewingArticle, setViewingArticle] = useState(null);
  const [editingArticle, setEditingArticle] = useState(null);
  const [viewMode, setViewMode] = useState('all'); // 'all' hoặc 'my' (chỉ staff)
  const [searchTerm, setSearchTerm] = useState('');
  const [formData, setFormData] = useState({
    newsTitle: '',
    headline: '',
    newsContent: '',
    newsSource: '',
    categoryId: '',
    newsStatus: true,
    tags: [],
  });
  const [tagInput, setTagInput] = useState('');

  useEffect(() => {
    loadData();
  }, [viewMode]);

  const loadData = async () => {
    try {
      setLoading(true);
      let articlesResponse;
      
      // Xác định endpoint dựa trên viewMode
      if (viewMode === 'my' && isStaff()) {
        articlesResponse = await newsArticleService.getMyArticles();
      } else {
        articlesResponse = await newsArticleService.getAll();
      }
      
      const categoriesResponse = await categoryService.getActive();
      
      console.log('Articles response:', articlesResponse);
      console.log('Categories response:', categoriesResponse);
      
      // Handle different response structures
      const articlesData = articlesResponse.data?.data || articlesResponse.data || articlesResponse;
      const categoriesData = categoriesResponse.data?.data || categoriesResponse.data || categoriesResponse;
      
      setArticles(Array.isArray(articlesData) ? articlesData : []);
      setCategories(Array.isArray(categoriesData) ? categoriesData : []);
      setError('');
    } catch (err) {
      console.error('Load data error:', err);
      setError(err.response?.data?.message || 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    // Chỉ Staff có thể tạo bài viết
    if (!isStaff()) {
      setError('Only Staff can create articles');
      return;
    }
    setEditingArticle(null);
    setFormData({
      newsTitle: '',
      headline: '',
      newsContent: '',
      newsSource: '',
      categoryId: '',
      newsStatus: true,
      tags: [],
    });
    setTagInput('');
    setShowModal(true);
  };

  const handleEdit = (article) => {
    setEditingArticle(article);
    setFormData({
      newsArticleId: article.newsArticleId,
      newsTitle: article.newsTitle,
      headline: article.headline || '',
      newsContent: article.newsContent || '',
      newsSource: article.newsSource || '',
      categoryId: article.categoryId,
      newsStatus: article.newsStatus,
      tags: article.tags?.map((t) => t.tagName) || [],
    });
    setTagInput('');
    setShowModal(true);
  };

  const handleView = (article) => {
    setViewingArticle(article);
    setShowViewModal(true);
  };

  const handleAddTag = () => {
    if (tagInput.trim() && !formData.tags.includes(tagInput.trim())) {
      setFormData({ ...formData, tags: [...formData.tags, tagInput.trim()] });
      setTagInput('');
    }
  };

  const handleRemoveTag = (tag) => {
    setFormData({ ...formData, tags: formData.tags.filter((t) => t !== tag) });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const submitData = {
        ...formData,
        categoryId: parseInt(formData.categoryId),
      };

      if (editingArticle) {
        await newsArticleService.update(editingArticle.newsArticleId, submitData);
      } else {
        await newsArticleService.create(submitData);
      }
      setShowModal(false);
      loadData();
    } catch (err) {
      setError(err.response?.data?.message || 'Operation failed');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this article?')) {
      try {
        await newsArticleService.delete(id);
        loadData();
      } catch (err) {
        setError(err.response?.data?.message || 'Failed to delete article');
      }
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      loadData();
      return;
    }
    try {
      setLoading(true);
      const response = await newsArticleService.search(searchTerm);
      const articlesData = response.data?.data || response.data || response;
      setArticles(Array.isArray(articlesData) ? articlesData : []);
      setError('');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to search articles');
      setArticles([]);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div>
      <div className="page-header">
        <h1>News Article Management</h1>
        <p>Manage news articles</p>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="card">
        <div className="button-group">
          {/* Search form */}
          <form onSubmit={handleSearch} style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
            <input
              type="text"
              placeholder="Search by article title or content..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              style={{ flex: 1, padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
            />
            <button type="submit" className="btn btn-primary">Search</button>
            <button 
              type="button" 
              onClick={() => {
                setSearchTerm('');
                loadData();
              }} 
              className="btn"
              style={{ background: '#6c757d', color: 'white' }}
            >
              Reset
            </button>
          </form>

          {/* Hiển thị tab View Mode cho Staff */}
          {isStaff() && (
            <div style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
              <button
                onClick={() => setViewMode('all')}
                className={`btn ${viewMode === 'all' ? 'btn-primary' : ''}`}
                style={{
                  background: viewMode === 'all' ? '#007bff' : '#e9ecef',
                  color: viewMode === 'all' ? 'white' : 'black',
                }}
              >
                All Articles
              </button>
              <button
                onClick={() => setViewMode('my')}
                className={`btn ${viewMode === 'my' ? 'btn-primary' : ''}`}
                style={{
                  background: viewMode === 'my' ? '#007bff' : '#e9ecef',
                  color: viewMode === 'my' ? 'white' : 'black',
                }}
              >
                My Articles
              </button>
            </div>
          )}
          
          {/* Chỉ Staff có nút Create */}
          {isStaff() && (
            <button onClick={handleCreate} className="btn btn-primary">
              + Create Article
            </button>
          )}
        </div>

        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Category</th>
                <th>Author</th>
                <th>Status</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {articles.map((article) => (
                <tr key={article.newsArticleId}>
                  <td>{article.newsArticleId}</td>
                  <td>{article.newsTitle}</td>
                  <td>{article.categoryName}</td>
                  <td>{article.createdByName}</td>
                  <td>
                    <span className={`status-badge ${article.newsStatus ? 'status-active' : 'status-inactive'}`}>
                      {article.newsStatus ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td>{new Date(article.createdDate).toLocaleDateString()}</td>
                  <td>
                    <button
                      onClick={() => navigate(`/article/${article.newsArticleId}`)}
                      className="btn btn-info"
                      style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                    >
                      View
                    </button>
                    {/* Staff: chỉ có thể edit/delete bài viết của chính mình */}
                    {isStaff() && article.createdById === user?.accountId && (
                      <>
                        <button
                          onClick={() => handleEdit(article)}
                          className="btn btn-warning"
                          style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                        >
                          Edit
                        </button>
                        <button
                          onClick={() => handleDelete(article.newsArticleId)}
                          className="btn btn-danger"
                          style={{ padding: '5px 10px', fontSize: '12px' }}
                        >
                          Delete
                        </button>
                      </>
                    )}
                    {/* Admin không có action (hidden) */}
                    {isAdmin() && (
                      <span style={{ color: '#999', fontSize: '12px' }}>View Only</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()} style={{ maxWidth: '700px' }}>
            <h2>{editingArticle ? 'Edit Article' : 'Create Article'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Title *</label>
                <input
                  type="text"
                  value={formData.newsTitle}
                  onChange={(e) => setFormData({ ...formData, newsTitle: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Headline</label>
                <input
                  type="text"
                  value={formData.headline}
                  onChange={(e) => setFormData({ ...formData, headline: e.target.value })}
                />
              </div>

              <div className="form-group">
                <label>Content *</label>
                <textarea
                  value={formData.newsContent}
                  onChange={(e) => setFormData({ ...formData, newsContent: e.target.value })}
                  required
                  style={{ minHeight: '150px' }}
                />
              </div>

              <div className="form-group">
                <label>Source</label>
                <input
                  type="text"
                  value={formData.newsSource}
                  onChange={(e) => setFormData({ ...formData, newsSource: e.target.value })}
                />
              </div>

              <div className="form-group">
                <label>Category *</label>
                <select
                  value={formData.categoryId}
                  onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                  required
                >
                  <option value="">Select Category</option>
                  {categories.map((cat) => (
                    <option key={cat.categoryId} value={cat.categoryId}>
                      {cat.categoryName}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label>Tags</label>
                <div style={{ display: 'flex', gap: '10px', marginBottom: '10px' }}>
                  <input
                    type="text"
                    value={tagInput}
                    onChange={(e) => setTagInput(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddTag())}
                    placeholder="Enter tag and press Enter"
                  />
                  <button type="button" onClick={handleAddTag} className="btn btn-primary">
                    Add
                  </button>
                </div>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '5px' }}>
                  {formData.tags.map((tag) => (
                    <span
                      key={tag}
                      style={{
                        background: '#e0e0e0',
                        padding: '5px 10px',
                        borderRadius: '15px',
                        fontSize: '12px',
                        display: 'flex',
                        alignItems: 'center',
                        gap: '5px',
                      }}
                    >
                      {tag}
                      <button
                        type="button"
                        onClick={() => handleRemoveTag(tag)}
                        style={{
                          background: 'none',
                          border: 'none',
                          cursor: 'pointer',
                          padding: '0',
                          fontSize: '16px',
                        }}
                      >
                        ×
                      </button>
                    </span>
                  ))}
                </div>
              </div>

              <div className="form-group">
                <label>
                  <input
                    type="checkbox"
                    checked={formData.newsStatus}
                    onChange={(e) => setFormData({ ...formData, newsStatus: e.target.checked })}
                    style={{ width: 'auto', marginRight: '10px' }}
                  />
                  Active
                </label>
              </div>

              <div className="form-actions">
                <button type="button" onClick={() => setShowModal(false)} className="btn">
                  Cancel
                </button>
                <button type="submit" className="btn btn-primary">
                  {editingArticle ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showViewModal && viewingArticle && (
        <div className="modal-overlay" onClick={() => setShowViewModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()} style={{ maxWidth: '800px', maxHeight: '80vh', overflow: 'auto' }}>
            <h2>Article Details</h2>
            <div style={{ padding: '20px' }}>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>ID:</label>
                <p>{viewingArticle.newsArticleId}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Title:</label>
                <p>{viewingArticle.newsTitle}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Headline:</label>
                <p>{viewingArticle.headline || 'N/A'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Content:</label>
                <p style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>{viewingArticle.newsContent || 'N/A'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Source:</label>
                <p>{viewingArticle.newsSource || 'N/A'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Category:</label>
                <p>{viewingArticle.categoryName || 'N/A'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Author:</label>
                <p>{viewingArticle.createdByName || 'N/A'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Status:</label>
                <p>{viewingArticle.newsStatus ? 'Active' : 'Inactive'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Created Date:</label>
                <p>{new Date(viewingArticle.createdDate).toLocaleString()}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Modified Date:</label>
                <p>{viewingArticle.modifiedDate ? new Date(viewingArticle.modifiedDate).toLocaleString() : 'N/A'}</p>
              </div>
              {viewingArticle.tags && viewingArticle.tags.length > 0 && (
                <div style={{ marginBottom: '15px' }}>
                  <label style={{ fontWeight: 'bold' }}>Tags:</label>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: '5px' }}>
                    {viewingArticle.tags.map((tag) => (
                      <span
                        key={tag.tagId}
                        style={{
                          background: '#007bff',
                          color: 'white',
                          padding: '5px 10px',
                          borderRadius: '15px',
                          fontSize: '12px',
                        }}
                      >
                        {tag.tagName}
                      </span>
                    ))}
                  </div>
                </div>
              )}
            </div>
            <div className="form-actions">
              <button type="button" onClick={() => setShowViewModal(false)} className="btn btn-primary">
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
