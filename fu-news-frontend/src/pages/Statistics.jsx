import { useState, useEffect } from 'react';
import { newsArticleService } from '../services/newsArticleService';

export default function Statistics() {
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filters, setFilters] = useState({
    fromDate: '',
    toDate: ''
  });

  useEffect(() => {
    loadAllArticles();
  }, []);

  const loadAllArticles = async () => {
    try {
      setLoading(true);
      const response = await newsArticleService.getAll();
      const data = response?.data?.data || response?.data || response || [];
      setArticles(Array.isArray(data) ? data : []);
      setError('');
    } catch (err) {
      console.error('Load articles error:', err);
      setError('Failed to load articles for report');
    } finally {
      setLoading(false);
    }
  };

  const getFilteredArticles = () => {
    let filtered = [...articles];

    if (filters.fromDate) {
      const fromDate = new Date(filters.fromDate);
      filtered = filtered.filter(a => new Date(a.createdDate) >= fromDate);
    }

    if (filters.toDate) {
      const toDate = new Date(filters.toDate);
      toDate.setHours(23, 59, 59, 999);
      filtered = filtered.filter(a => new Date(a.createdDate) <= toDate);
    }

    // Sort by date descending
    filtered.sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate));

    return filtered;
  };

  const getStatistics = () => {
    const filtered = getFilteredArticles();
    
    const stats = {
      totalArticles: filtered.length,
      activeArticles: filtered.filter(a => a.newsStatus).length,
      inactiveArticles: filtered.filter(a => !a.newsStatus).length,
      byCategory: {},
      byAuthor: {}
    };

    filtered.forEach(article => {
      // By category
      if (!stats.byCategory[article.categoryName]) {
        stats.byCategory[article.categoryName] = 0;
      }
      stats.byCategory[article.categoryName]++;

      // By author
      if (!stats.byAuthor[article.createdByName]) {
        stats.byAuthor[article.createdByName] = 0;
      }
      stats.byAuthor[article.createdByName]++;
    });

    return stats;
  };

  const handleReset = () => {
    setFilters({ fromDate: '', toDate: '' });
  };

  const handleExportCSV = () => {
    const filtered = getFilteredArticles();
    let csv = 'ID,Title,Author,Category,Status,Created Date\n';
    
    filtered.forEach(article => {
      csv += `${article.newsArticleId},"${article.newsTitle}","${article.createdByName}","${article.categoryName}","${article.newsStatus ? 'Active' : 'Inactive'}","${new Date(article.createdDate).toLocaleDateString()}"\n`;
    });

    const blob = new Blob([csv], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `news_report_${new Date().toISOString().split('T')[0]}.csv`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  };

  const stats = getStatistics();

  if (loading) {
    return <div className="loading">Loading articles for report...</div>;
  }

  return (
    <div>
      <div className="page-header">
        <h1>News Statistics Report</h1>
        <p>Analyze news articles by date range</p>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="card" style={{ marginBottom: '20px' }}>
        <h2>Filter by Date Range</h2>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '15px', marginTop: '15px' }}>
          <div>
            <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>From Date</label>
            <input
              type="date"
              value={filters.fromDate}
              onChange={(e) => setFilters({ ...filters, fromDate: e.target.value })}
              style={{ width: '100%', padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
            />
          </div>

          <div>
            <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>To Date</label>
            <input
              type="date"
              value={filters.toDate}
              onChange={(e) => setFilters({ ...filters, toDate: e.target.value })}
              style={{ width: '100%', padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
            />
          </div>

          <div style={{ display: 'flex', gap: '10px', alignItems: 'flex-end' }}>
            <button 
              onClick={handleReset}
              className="btn"
              style={{ background: '#6c757d', color: 'white', flex: 1 }}
            >
              Reset
            </button>
            <button 
              onClick={handleExportCSV}
              className="btn btn-primary"
              style={{ flex: 1 }}
            >
              Export CSV
            </button>
          </div>
        </div>
      </div>

      {/* Summary Stats */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '15px', marginBottom: '20px' }}>
        <div style={{ padding: '20px', background: '#3498db', color: 'white', borderRadius: '8px' }}>
          <h3 style={{ margin: '0 0 10px', fontSize: '16px' }}>Total Articles</h3>
          <p style={{ fontSize: '32px', margin: 0, fontWeight: 'bold' }}>{stats.totalArticles}</p>
        </div>

        <div style={{ padding: '20px', background: '#2ecc71', color: 'white', borderRadius: '8px' }}>
          <h3 style={{ margin: '0 0 10px', fontSize: '16px' }}>Active</h3>
          <p style={{ fontSize: '32px', margin: 0, fontWeight: 'bold' }}>{stats.activeArticles}</p>
        </div>

        <div style={{ padding: '20px', background: '#e74c3c', color: 'white', borderRadius: '8px' }}>
          <h3 style={{ margin: '0 0 10px', fontSize: '16px' }}>Inactive</h3>
          <p style={{ fontSize: '32px', margin: 0, fontWeight: 'bold' }}>{stats.inactiveArticles}</p>
        </div>
      </div>

      {/* Breakdown by Category */}
      <div className="card" style={{ marginBottom: '20px' }}>
        <h2>Articles by Category</h2>
        {Object.keys(stats.byCategory).length === 0 ? (
          <p style={{ color: '#999' }}>No data</p>
        ) : (
          <div style={{ marginTop: '15px' }}>
            {Object.entries(stats.byCategory)
              .sort(([, a], [, b]) => b - a)
              .map(([category, count]) => (
                <div key={category} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px', borderBottom: '1px solid #eee' }}>
                  <span>{category}</span>
                  <span style={{ fontWeight: 'bold', background: '#e9ecef', padding: '5px 10px', borderRadius: '4px' }}>{count}</span>
                </div>
              ))}
          </div>
        )}
      </div>

      {/* Breakdown by Author */}
      <div className="card" style={{ marginBottom: '20px' }}>
        <h2>Articles by Author</h2>
        {Object.keys(stats.byAuthor).length === 0 ? (
          <p style={{ color: '#999' }}>No data</p>
        ) : (
          <div style={{ marginTop: '15px' }}>
            {Object.entries(stats.byAuthor)
              .sort(([, a], [, b]) => b - a)
              .map(([author, count]) => (
                <div key={author} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px', borderBottom: '1px solid #eee' }}>
                  <span>{author}</span>
                  <span style={{ fontWeight: 'bold', background: '#e9ecef', padding: '5px 10px', borderRadius: '4px' }}>{count}</span>
                </div>
              ))}
          </div>
        )}
      </div>

      {/* Detailed Article List */}
      <div className="card">
        <h2>Detailed Article List (Sorted by Date - Descending)</h2>
        {getFilteredArticles().length === 0 ? (
          <p style={{ color: '#999', textAlign: 'center', padding: '20px' }}>No articles found for the selected date range</p>
        ) : (
          <div className="table-container" style={{ marginTop: '15px' }}>
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Title</th>
                  <th>Author</th>
                  <th>Category</th>
                  <th>Status</th>
                  <th>Created Date</th>
                </tr>
              </thead>
              <tbody>
                {getFilteredArticles().map((article) => (
                  <tr key={article.newsArticleId}>
                    <td>{article.newsArticleId}</td>
                    <td style={{ maxWidth: '300px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                      {article.newsTitle}
                    </td>
                    <td>{article.createdByName}</td>
                    <td>{article.categoryName}</td>
                    <td>
                      <span className={`status-badge ${article.newsStatus ? 'status-active' : 'status-inactive'}`}>
                        {article.newsStatus ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td>
                      {new Date(article.createdDate).toLocaleDateString('vi-VN', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit'
                      })}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
