import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { newsArticleService } from '../services/newsArticleService';

export default function PublicNews() {
  const navigate = useNavigate();
  const { user, logout, isAdmin, isStaff } = useAuth();
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      
      // Load active news
      const newsRes = await newsArticleService.getActive().catch(() => ({ data: [] }));
      
      const newsData = newsRes?.data?.data || newsRes?.data || newsRes || [];
      
      setArticles(Array.isArray(newsData) ? newsData : []);
      setError('');
    } catch (err) {
      console.error('Load data error:', err);
      setError('Failed to load news');
    } finally {
      setLoading(false);
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
      
      // Search articles by keyword
      const searchRes = await newsArticleService.search(searchTerm.trim()).catch(() => ({ data: [] }));
      let newsData = searchRes?.data?.data || searchRes?.data || searchRes || [];
      
      newsData = Array.isArray(newsData) ? newsData : [];
      
      setArticles(newsData);
      setError('');
    } catch (err) {
      console.error('Search error:', err);
      setError('Search failed');
      setArticles([]);
    } finally {
      setLoading(false);
    }
  };

  const handleReset = () => {
    setSearchTerm('');
    loadData();
  };

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  if (loading && articles.length === 0) {
    return <div className="loading">Loading news...</div>;
  }

  return (
    <div>
      <div className="page-header">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h1>News & Updates</h1>
            <p>Latest news from our university</p>
          </div>
          <div style={{ display: 'flex', gap: '10px' }}>
            {user ? (
              <>
                <span style={{ alignSelf: 'center', color: '#666', fontSize: '14px' }}>
                  {user.accountName} ({isAdmin() ? 'Admin' : isStaff() ? 'Staff' : 'Lecturer'})
                </span>
                <button
                  onClick={handleLogout}
                  className="btn btn-danger"
                  style={{ height: 'fit-content' }}
                >
                  Logout
                </button>
              </>
            ) : (
              <button
                onClick={() => navigate('/login')}
                className="btn btn-primary"
                style={{ height: 'fit-content' }}
              >
                Login
              </button>
            )}
          </div>
        </div>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="card">
        <form onSubmit={handleSearch} style={{ marginBottom: '20px' }}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr auto auto', gap: '10px', marginBottom: '15px' }}>
            <div>
              <input
                type="text"
                placeholder="Search articles..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                style={{ width: '100%', padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
              />
            </div>

            <button type="submit" className="btn btn-primary">
              Search
            </button>

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

        {articles.length === 0 ? (
          <div style={{ textAlign: 'center', padding: '40px' }}>
            <p style={{ fontSize: '18px', color: '#999' }}>No articles found</p>
          </div>
        ) : (
          <div style={{ display: 'grid', gap: '20px' }}>
            {articles.map((article) => (
              <div
                key={article.newsArticleId}
                style={{
                  border: '1px solid #ddd',
                  borderRadius: '8px',
                  padding: '20px',
                  cursor: 'pointer',
                  transition: 'all 0.3s ease',
                  background: 'white'
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.background = '#f9f9f9';
                  e.currentTarget.style.boxShadow = '0 2px 8px rgba(0,0,0,0.1)';
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.background = 'white';
                  e.currentTarget.style.boxShadow = 'none';
                }}
              >
                <div 
                  onClick={() => navigate(`/article/${article.newsArticleId}`)}
                  style={{ cursor: 'pointer' }}
                >
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '10px' }}>
                    <div>
                      <h3 style={{ margin: '0 0 8px', fontSize: '20px', fontWeight: '600' }}>
                        {article.newsTitle}
                      </h3>
                      {article.headline && (
                        <p style={{ margin: '0 0 10px', color: '#666', fontSize: '14px' }}>
                          {article.headline}
                        </p>
                      )}
                    </div>
                  </div>

                  <p style={{
                    margin: '0 0 15px',
                    color: '#555',
                    fontSize: '14px',
                    lineHeight: '1.6',
                    display: '-webkit-box',
                    WebkitLineClamp: 3,
                    WebkitBoxOrient: 'vertical',
                    overflow: 'hidden'
                  }}>
                    {article.newsContent}
                  </p>

                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '10px', fontSize: '13px', color: '#999' }}>
                    <div style={{ display: 'flex', gap: '15px' }}>
                      <span>
                        <strong>Category:</strong> {article.categoryName}
                      </span>
                      <span>
                        <strong>Author:</strong> {article.createdByName}
                      </span>
                      <span>
                        <strong>Date:</strong> {new Date(article.createdDate).toLocaleDateString('vi-VN')}
                      </span>
                    </div>

                    {article.tags && article.tags.length > 0 && (
                      <div style={{ display: 'flex', gap: '5px' }}>
                        {article.tags.slice(0, 3).map((tag, idx) => (
                          <span
                            key={idx}
                            style={{
                              background: '#e0e0e0',
                              padding: '3px 8px',
                              borderRadius: '10px',
                              fontSize: '11px'
                            }}
                          >
                            {tag.tagName || tag}
                          </span>
                        ))}
                        {article.tags.length > 3 && (
                          <span style={{ color: '#999' }}>+{article.tags.length - 3}</span>
                        )}
                      </div>
                    )}
                  </div>
                </div>

                <button
                  onClick={() => navigate(`/article/${article.newsArticleId}`)}
                  className="btn btn-primary"
                  style={{ marginTop: '15px', width: '100%' }}
                >
                  Read More →
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
