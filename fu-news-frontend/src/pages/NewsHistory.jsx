import { useState, useEffect } from 'react';
import { newsArticleService } from '../services/newsArticleService';
import { useAuth } from '../contexts/AuthContext';

export default function NewsHistory() {
  const { user } = useAuth();
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filters, setFilters] = useState({
    searchTerm: '',
    status: '' // 'active', 'inactive', or ''
  });

  useEffect(() => {
    loadMyArticles();
  }, []);

  const loadMyArticles = async () => {
    try {
      setLoading(true);
      const response = await newsArticleService.getMyArticles();
      const data = response?.data?.data || response?.data || response || [];
      setArticles(Array.isArray(data) ? data : []);
      setError('');
    } catch (err) {
      console.error('Load my articles error:', err);
      setError('Failed to load your articles');
    } finally {
      setLoading(false);
    }
  };

  const getFilteredArticles = () => {
    let filtered = [...articles];

    // Filter by search term
    if (filters.searchTerm) {
      const search = filters.searchTerm.toLowerCase();
      filtered = filtered.filter(a => 
        a.newsTitle.toLowerCase().includes(search) ||
        a.newsContent.toLowerCase().includes(search)
      );
    }

    // Filter by status
    if (filters.status) {
      const isActive = filters.status === 'active';
      filtered = filtered.filter(a => a.newsStatus === isActive);
    }

    // Sort by date descending
    filtered.sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate));

    return filtered;
  };

  const stats = {
    total: articles.length,
    active: articles.filter(a => a.newsStatus).length,
    inactive: articles.filter(a => !a.newsStatus).length
  };

  const handleReset = () => {
    setFilters({ searchTerm: '', status: '' });
  };

  if (loading) {
    return <div className="loading">Loading your articles...</div>;
  }

  return (
    <div>
      <div className="page-header">
        <h1>My News History</h1>
        <p>View and manage all articles you created</p>
      </div>

      {error && <div className="error-message">{error}</div>}

      {/* Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))', gap: '15px', marginBottom: '20px' }}>
        <div style={{ padding: '15px', background: '#3498db', color: 'white', borderRadius: '8px', textAlign: 'center' }}>
          <p style={{ margin: '0 0 8px', fontSize: '13px' }}>Total Articles</p>
          <p style={{ fontSize: '28px', margin: 0, fontWeight: 'bold' }}>{stats.total}</p>
        </div>

        <div style={{ padding: '15px', background: '#2ecc71', color: 'white', borderRadius: '8px', textAlign: 'center' }}>
          <p style={{ margin: '0 0 8px', fontSize: '13px' }}>Active</p>
          <p style={{ fontSize: '28px', margin: 0, fontWeight: 'bold' }}>{stats.active}</p>
        </div>

        <div style={{ padding: '15px', background: '#e74c3c', color: 'white', borderRadius: '8px', textAlign: 'center' }}>
          <p style={{ margin: '0 0 8px', fontSize: '13px' }}>Inactive</p>
          <p style={{ fontSize: '28px', margin: 0, fontWeight: 'bold' }}>{stats.inactive}</p>
        </div>
      </div>

      <div className="card">
        <h2>Filter Articles</h2>
        <form onSubmit={(e) => e.preventDefault()} style={{ marginBottom: '20px' }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '10px' }}>
            <div>
              <input
                type="text"
                placeholder="Search by title or content..."
                value={filters.searchTerm}
                onChange={(e) => setFilters({ ...filters, searchTerm: e.target.value })}
                style={{ width: '100%', padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
              />
            </div>

            <div>
              <select
                value={filters.status}
                onChange={(e) => setFilters({ ...filters, status: e.target.value })}
                style={{ width: '100%', padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
              >
                <option value="">All Status</option>
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
              </select>
            </div>

            <button 
              type="button"
              onClick={handleReset}
              className="btn"
              style={{ background: '#6c757d', color: 'white' }}
            >
              Reset
            </button>
          </div>
        </form>

        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Category</th>
                <th>Status</th>
                <th>Created</th>
                <th>Modified</th>
              </tr>
            </thead>
            <tbody>
              {getFilteredArticles().length === 0 ? (
                <tr>
                  <td colSpan="6" style={{ textAlign: 'center', padding: '20px', color: '#999' }}>
                    No articles found
                  </td>
                </tr>
              ) : (
                getFilteredArticles().map((article) => (
                  <tr key={article.newsArticleId}>
                    <td>{article.newsArticleId}</td>
                    <td style={{ maxWidth: '250px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                      {article.newsTitle}
                    </td>
                    <td>{article.categoryName}</td>
                    <td>
                      <span className={`status-badge ${article.newsStatus ? 'status-active' : 'status-inactive'}`}>
                        {article.newsStatus ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td>{new Date(article.createdDate).toLocaleDateString('vi-VN')}</td>
                    <td>{article.modifiedDate ? new Date(article.modifiedDate).toLocaleDateString('vi-VN') : '-'}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
