import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { accountService } from '../services/accountService';
import { categoryService } from '../services/categoryService';
import { newsArticleService } from '../services/newsArticleService';

export default function Home() {
  const { user, isAdmin, isStaff } = useAuth();
  const navigate = useNavigate();
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalCategories: 0,
    totalNews: 0,
    loading: true
  });

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      const promises = [];
      
      if (isAdmin()) {
        promises.push(
          accountService.getAll().catch(() => ({ data: [] })),
          categoryService.getAll().catch(() => ({ data: [] })),
          newsArticleService.getAll().catch(() => ({ data: [] }))
        );
      } else if (isStaff()) {
        promises.push(
          Promise.resolve({ data: [] }),
          categoryService.getAll().catch(() => ({ data: [] })),
          newsArticleService.getAll().catch(() => ({ data: [] }))
        );
      } else {
        setStats({ totalUsers: 0, totalCategories: 0, totalNews: 0, loading: false });
        return;
      }

      const [accountsRes, categoriesRes, newsRes] = await Promise.all(promises);
      
      const accountsData = accountsRes?.data?.data || accountsRes?.data || accountsRes || [];
      const categoriesData = categoriesRes?.data?.data || categoriesRes?.data || categoriesRes || [];
      const newsData = newsRes?.data?.data || newsRes?.data || newsRes || [];
      
      setStats({
        totalUsers: Array.isArray(accountsData) ? accountsData.length : 0,
        totalCategories: Array.isArray(categoriesData) ? categoriesData.length : 0,
        totalNews: Array.isArray(newsData) ? newsData.length : 0,
        loading: false
      });
    } catch (error) {
      console.error('Error loading stats:', error);
      setStats({ totalUsers: 0, totalCategories: 0, totalNews: 0, loading: false });
    }
  };

  const getRoleName = () => {
    if (!user) return 'Guest';
    const roleNames = { 0: 'Admin', 1: 'Staff', 2: 'Lecturer',  };
    return roleNames[user.accountRole] || 'Unknown';
  };

  return (
    <div>
      <div className="page-header">
        <h1>Welcome, {user?.accountName}!</h1>
        <p>FU News Management System Dashboard</p>
      </div>

      <div className="card">
        <h2>Quick Stats</h2>
        {stats.loading ? (
          <div style={{ padding: '20px', textAlign: 'center' }}>Loading stats...</div>
        ) : (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '20px', marginTop: '20px' }}>
            {isAdmin() && (
              <div style={{ padding: '20px', background: '#3498db', color: 'white', borderRadius: '8px' }}>
                <h3 style={{ margin: '0 0 10px' }}>Total Users</h3>
                <p style={{ fontSize: '32px', margin: 0, fontWeight: 'bold' }}>{stats.totalUsers}</p>
              </div>
            )}
            
            {(isAdmin() || isStaff()) && (
              <>
                <div style={{ padding: '20px', background: '#2ecc71', color: 'white', borderRadius: '8px' }}>
                  <h3 style={{ margin: '0 0 10px' }}>Categories</h3>
                  <p style={{ fontSize: '32px', margin: 0, fontWeight: 'bold' }}>{stats.totalCategories}</p>
                </div>
                
                <div style={{ padding: '20px', background: '#f39c12', color: 'white', borderRadius: '8px' }}>
                  <h3 style={{ margin: '0 0 10px' }}>News Articles</h3>
                  <p style={{ fontSize: '32px', margin: 0, fontWeight: 'bold' }}>{stats.totalNews}</p>
                </div>
              </>
            )}
          </div>
        )}
      </div>

      <div className="card">
        <h2>System Information</h2>
        <p><strong>Role:</strong> {getRoleName()}</p>
        <p><strong>Email:</strong> {user?.accountEmail}</p>
        <p><strong>Account ID:</strong> {user?.accountId}</p>
      </div>

      {!user && (
        <div className="card" style={{ background: '#f0f8ff', borderLeft: '4px solid #007bff' }}>
          <h2 style={{ marginTop: 0 }}>Want to Manage News?</h2>
          <p>Sign in to access the management dashboard for news, categories, and more.</p>
          <button 
            onClick={() => navigate('/login')}
            className="btn btn-primary"
            style={{ marginTop: '10px' }}
          >
            Go to Login
          </button>
        </div>
      )}
    </div>
  );
}
