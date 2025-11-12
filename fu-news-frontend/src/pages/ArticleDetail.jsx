import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { newsArticleService } from '../services/newsArticleService';
import { useAuth } from '../contexts/AuthContext';

export default function ArticleDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user, isStaff, isAdmin } = useAuth();
  
  const [article, setArticle] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadArticle();
  }, [id]);

  const loadArticle = async () => {
    try {
      setLoading(true);
      const response = await newsArticleService.getById(id);
      const data = response?.data?.data || response?.data || response;
      
      // Check if article is active or user is staff
      if (!data) {
        setError('Article not found');
      } else if (!data.newsStatus && !isStaff()) {
        setError('This article is not available');
      } else {
        setArticle(data);
      }
      setError('');
    } catch (err) {
      console.error('Load article error:', err);
      setError(err.response?.data?.message || 'Failed to load article');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading article...</div>;
  }

  if (error) {
    return (
      <div>
        <div className="page-header">
          <h1>News Article</h1>
        </div>
        <div className="card" style={{ textAlign: 'center' }}>
          <div className="error-message" style={{ marginBottom: '20px' }}>{error}</div>
          <button 
            onClick={() => {
              if (isAdmin() || isStaff()) {
                navigate('/dashboard');
              } else {
                navigate('/news');
              }
            }} 
            className="btn btn-primary"
          >
            Back
          </button>
        </div>
      </div>
    );
  }

  if (!article) {
    return (
      <div>
        <div className="page-header">
          <h1>News Article</h1>
        </div>
        <div className="card">
          <p>Article not found</p>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div className="page-header">
        <h1>News Article</h1>
        <button 
          onClick={() => {
            // If admin or staff, go back to dashboard
            if (isAdmin() || isStaff()) {
              navigate('/dashboard/news');
            } else {
              // Otherwise go back to news list
              navigate('/news');
            }
          }} 
          className="btn" 
          style={{ background: '#6c757d', color: 'white' }}
        >
          ← Back
        </button>
      </div>

      <div className="card" style={{ maxWidth: '900px', margin: '20px auto' }}>
        <div style={{ marginBottom: '20px' }}>
          <h1 style={{ marginBottom: '10px' }}>{article.newsTitle}</h1>
          
          {article.headline && (
            <p style={{ fontSize: '18px', color: '#666', fontStyle: 'italic', marginBottom: '15px' }}>
              {article.headline}
            </p>
          )}

          <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap', marginBottom: '20px', paddingBottom: '20px', borderBottom: '1px solid #ddd' }}>
            <div>
              <strong>Category:</strong>
              <p style={{ margin: '5px 0' }}>{article.categoryName || '-'}</p>
            </div>
            
            <div>
              <strong>Author:</strong>
              <p style={{ margin: '5px 0' }}>{article.createdByName || 'Unknown'}</p>
            </div>
            
            <div>
              <strong>Published:</strong>
              <p style={{ margin: '5px 0' }}>
                {new Date(article.createdDate).toLocaleDateString('vi-VN', {
                  year: 'numeric',
                  month: '2-digit',
                  day: '2-digit',
                  hour: '2-digit',
                  minute: '2-digit'
                })}
              </p>
            </div>

            {article.newsSource && (
              <div>
                <strong>Source:</strong>
                <p style={{ margin: '5px 0' }}>{article.newsSource}</p>
              </div>
            )}

            <div>
              <strong>Status:</strong>
              <p style={{ margin: '5px 0' }}>
                <span className={`status-badge ${article.newsStatus ? 'status-active' : 'status-inactive'}`}>
                  {article.newsStatus ? 'Active' : 'Inactive'}
                </span>
              </p>
            </div>
          </div>

          {/* Tags */}
          {article.tags && article.tags.length > 0 && (
            <div style={{ marginBottom: '20px' }}>
              <strong>Tags:</strong>
              <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', marginTop: '10px' }}>
                {article.tags.map((tag, idx) => (
                  <span
                    key={idx}
                    style={{
                      background: '#e0e0e0',
                      padding: '5px 12px',
                      borderRadius: '15px',
                      fontSize: '13px'
                    }}
                  >
                    {tag.tagName || tag}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Content */}
          <div style={{ marginTop: '30px', lineHeight: '1.8', fontSize: '16px' }}>
            <div style={{ whiteSpace: 'pre-wrap' }}>
              {article.newsContent}
            </div>
          </div>

          {/* Modified info */}
          {article.modifiedDate && (
            <div style={{ marginTop: '30px', paddingTop: '20px', borderTop: '1px solid #ddd', color: '#999', fontSize: '13px' }}>
              <p>Last updated: {new Date(article.modifiedDate).toLocaleDateString('vi-VN')}</p>
            </div>
          )}
        </div>

        {/* Action buttons for staff */}
        {(isStaff() || isAdmin()) && (
          <div style={{ marginTop: '20px', display: 'flex', gap: '10px', justifyContent: 'center' }}>
            <button 
              onClick={() => navigate(`/dashboard/news`)}
              className="btn btn-primary"
            >
              Back to Dashboard
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
