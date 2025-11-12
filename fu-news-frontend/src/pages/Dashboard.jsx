import { Outlet, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Dashboard.css';

export default function Dashboard() {
  const { user, logout, isAdmin, isStaff } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <div className="dashboard">
      <aside className="sidebar">
        <div className="sidebar-header">
          <h2>FU News</h2>
          <p className="user-info">
            {user?.accountName}
            <span className="user-role">
              {isAdmin() ? 'Admin' : isStaff() ? 'Staff' : 'User'}
            </span>
          </p>
        </div>
        
        <nav className="sidebar-nav">
          <Link to="/dashboard" className="nav-link">
            <span>📊</span> Dashboard
          </Link>
          
          {isAdmin() && (
            <>
              <Link to="/dashboard/accounts" className="nav-link">
                <span>👥</span> Accounts
              </Link>
              
              <Link to="/dashboard/statistics" className="nav-link">
                <span>📈</span> Statistics Report
              </Link>
            </>
          )}
          
          {(isAdmin() || isStaff()) && (
            <>
              <Link to="/dashboard/categories" className="nav-link">
                <span>📁</span> Categories
              </Link>
              
              <Link to="/dashboard/news" className="nav-link">
                <span>📰</span> News Articles
              </Link>
            </>
          )}

          {isStaff() && (
            <Link to="/dashboard/news-history" className="nav-link">
              <span>📚</span> My News History
            </Link>
          )}

          <Link to="/dashboard/profile" className="nav-link">
            <span>⚙️</span> My Profile
          </Link>
          
          <button onClick={handleLogout} className="nav-link logout-btn">
            <span>🚪</span> Logout
          </button>
        </nav>
      </aside>
      
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
