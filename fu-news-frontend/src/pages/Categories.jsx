import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { categoryService } from '../services/categoryService';
import { CategoryTreeView } from '../components/CategoryTreeView';

export default function Categories() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [showViewModal, setShowViewModal] = useState(false);
  const [viewingCategory, setViewingCategory] = useState(null);
  const [editingCategory, setEditingCategory] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [viewMode, setViewMode] = useState('tree'); // 'tree' or 'table'
  const [formData, setFormData] = useState({
    categoryName: '',
    categoryDescrip: '',
    parentCategoryId: '',
    isActive: true,
  });

  useEffect(() => {
    loadCategories();
  }, []);

  const loadCategories = async () => {
    try {
      setLoading(true);
      const response = await categoryService.getAll();
      console.log('Full Categories response:', response);
      
      // Service trả về raw data từ axios: { success: true, message: "...", data: [...], errors: [] }
      // response ở đây đã là object, không cần truy cập .data của axios
      let data = [];
      if (response && response.data && Array.isArray(response.data)) {
        // Nếu response có .data property là array
        data = response.data;
      } else if (Array.isArray(response)) {
        // Nếu response trực tiếp là array
        data = response;
      }
      
      console.log('Parsed data:', data);
      setCategories(data);
      setError('');
    } catch (err) {
      console.error('Load categories error:', err);
      setError(err.response?.data?.message || 'Failed to load categories');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingCategory(null);
    setFormData({
      categoryName: '',
      categoryDescrip: '',
      parentCategoryId: '',
      isActive: true,
    });
    setShowModal(true);
  };

  const handleEdit = (category) => {
    setEditingCategory(category);
    setFormData({
      categoryId: category.categoryId,
      categoryName: category.categoryName,
      categoryDescrip: category.categoryDescrip || '',
      parentCategoryId: category.parentCategoryId || '',
      isActive: category.isActive,
    });
    setShowModal(true);
  };

  const handleView = (category) => {
    setViewingCategory(category);
    setShowViewModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const submitData = {
        ...formData,
        parentCategoryId: formData.parentCategoryId ? parseInt(formData.parentCategoryId) : null,
      };

      if (editingCategory) {
        await categoryService.update(editingCategory.categoryId, submitData);
      } else {
        await categoryService.create(submitData);
      }
      setShowModal(false);
      loadCategories();
    } catch (err) {
      setError(err.response?.data?.message || 'Operation failed');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this category?')) {
      try {
        await categoryService.delete(id);
        loadCategories();
      } catch (err) {
        setError(err.response?.data?.message || 'Failed to delete category');
      }
    }
  };

  const handleToggleStatus = async (id) => {
    try {
      await categoryService.toggleStatus(id);
      loadCategories();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to toggle status');
    }
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      loadCategories();
      return;
    }
    try {
      setLoading(true);
      const response = await categoryService.search(searchTerm);
      const data = response && response.data ? response.data : response;
      setCategories(Array.isArray(data) ? data : []);
      setError('');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to search categories');
      setCategories([]);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div>
      <div className="page-header">
        <h1>Category Management</h1>
        <p>Manage news categories</p>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="card">
        <div className="button-group">
          <form onSubmit={handleSearch} style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
            <input
              type="text"
              placeholder="Search by category name or description..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              style={{ flex: 1, padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
            />
            <button type="submit" className="btn btn-primary">Search</button>
            <button 
              type="button" 
              onClick={() => {
                setSearchTerm('');
                loadCategories();
              }} 
              className="btn"
              style={{ background: '#6c757d', color: 'white' }}
            >
              Reset
            </button>
          </form>
          <div style={{ display: 'flex', gap: '10px' }}>
            {searchTerm === '' && (
              <>
                <button 
                  onClick={() => setViewMode('tree')}
                  className="btn"
                  style={{ 
                    background: viewMode === 'tree' ? '#007bff' : '#e9ecef',
                    color: viewMode === 'tree' ? 'white' : '#333',
                    border: 'none'
                  }}
                  title="Tree View"
                >
                  🌳 Tree
                </button>
                <button 
                  onClick={() => setViewMode('table')}
                  className="btn"
                  style={{ 
                    background: viewMode === 'table' ? '#007bff' : '#e9ecef',
                    color: viewMode === 'table' ? 'white' : '#333',
                    border: 'none'
                  }}
                  title="Table View"
                >
                  📋 Table
                </button>
              </>
            )}
            <button onClick={handleCreate} className="btn btn-primary">
              + Create Category
            </button>
          </div>
        </div>

        {searchTerm !== '' ? (
          // Table view for search results
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Description</th>
                  <th>Parent</th>
                  <th>Status</th>
                  <th>News Count</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {categories.map((category) => (
                  <tr key={category.categoryId}>
                    <td>{category.categoryId}</td>
                    <td>{category.categoryName}</td>
                    <td>{category.categoryDescrip || '-'}</td>
                    <td>{category.parentCategoryName || '-'}</td>
                    <td>
                      <span className={`status-badge ${category.isActive ? 'status-active' : 'status-inactive'}`}>
                        {category.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td>{category.newsArticleCount}</td>
                    <td>
                      <button
                        onClick={() => handleView(category)}
                        className="btn btn-info"
                        style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                      >
                        View
                      </button>
                      <button
                        onClick={() => handleEdit(category)}
                        className="btn btn-warning"
                        style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleToggleStatus(category.categoryId)}
                        className="btn btn-primary"
                        style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                      >
                        Toggle
                      </button>
                      <button
                        onClick={() => handleDelete(category.categoryId)}
                        className="btn btn-danger"
                        style={{ 
                          padding: '5px 10px', 
                          fontSize: '12px',
                          opacity: category.canDelete ? 1 : 0.5,
                          cursor: category.canDelete ? 'pointer' : 'not-allowed'
                        }}
                        disabled={!category.canDelete}
                        title={!category.canDelete ? `Cannot delete: ${category.newsArticleCount > 0 ? 'Has news articles' : 'Has sub-categories'}` : 'Delete category'}
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : viewMode === 'tree' ? (
          // Tree view
          <CategoryTreeView
            onView={handleView}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onToggleStatus={handleToggleStatus}
            onRefresh={loadCategories}
          />
        ) : (
          // Table view
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Description</th>
                  <th>Parent</th>
                  <th>Status</th>
                  <th>News Count</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {categories.map((category) => (
                  <tr key={category.categoryId}>
                    <td>{category.categoryId}</td>
                    <td>{category.categoryName}</td>
                    <td>{category.categoryDescrip || '-'}</td>
                    <td>{category.parentCategoryName || '-'}</td>
                    <td>
                      <span className={`status-badge ${category.isActive ? 'status-active' : 'status-inactive'}`}>
                        {category.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td>{category.newsArticleCount}</td>
                    <td>
                      <button
                        onClick={() => handleView(category)}
                        className="btn btn-info"
                        style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                      >
                        View
                      </button>
                      <button
                        onClick={() => handleEdit(category)}
                        className="btn btn-warning"
                        style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleToggleStatus(category.categoryId)}
                        className="btn btn-primary"
                        style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
                      >
                        Toggle
                      </button>
                      <button
                        onClick={() => handleDelete(category.categoryId)}
                        className="btn btn-danger"
                        style={{ 
                          padding: '5px 10px', 
                          fontSize: '12px',
                          opacity: category.canDelete ? 1 : 0.5,
                          cursor: category.canDelete ? 'pointer' : 'not-allowed'
                        }}
                        disabled={!category.canDelete}
                        title={!category.canDelete ? `Cannot delete: ${category.newsArticleCount > 0 ? 'Has news articles' : 'Has sub-categories'}` : 'Delete category'}
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h2>{editingCategory ? 'Edit Category' : 'Create Category'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Category Name *</label>
                <input
                  type="text"
                  value={formData.categoryName}
                  onChange={(e) => setFormData({ ...formData, categoryName: e.target.value })}
                  required
                  maxLength={100}
                />
              </div>

              <div className="form-group">
                <label>Description</label>
                <textarea
                  value={formData.categoryDescrip}
                  onChange={(e) => setFormData({ ...formData, categoryDescrip: e.target.value })}
                  maxLength={500}
                />
              </div>

              <div className="form-group">
                <label>Parent Category</label>
                <select
                  value={formData.parentCategoryId}
                  onChange={(e) => setFormData({ ...formData, parentCategoryId: e.target.value })}
                >
                  <option value="">None (Root Category)</option>
                  {categories
                    .filter((cat) => !editingCategory || cat.categoryId !== editingCategory.categoryId)
                    .map((cat) => (
                      <option key={cat.categoryId} value={cat.categoryId}>
                        {cat.categoryName}
                      </option>
                    ))}
                </select>
              </div>

              <div className="form-group">
                <label>
                  <input
                    type="checkbox"
                    checked={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
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
                  {editingCategory ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showViewModal && viewingCategory && (
        <div className="modal-overlay" onClick={() => setShowViewModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h2>Category Details</h2>
            <div style={{ padding: '20px' }}>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>ID:</label>
                <p>{viewingCategory.categoryId}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Name:</label>
                <p>{viewingCategory.categoryName}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Description:</label>
                <p>{viewingCategory.categoryDescrip || 'N/A'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Parent Category:</label>
                <p>{viewingCategory.parentCategoryName || 'None (Root)'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Status:</label>
                <p>{viewingCategory.isActive ? 'Active' : 'Inactive'}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>News Articles Count:</label>
                <p>{viewingCategory.newsArticleCount}</p>
              </div>
              <div style={{ marginBottom: '15px' }}>
                <label style={{ fontWeight: 'bold' }}>Can Delete:</label>
                <p>{viewingCategory.canDelete ? 'Yes' : 'No'}</p>
              </div>
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
