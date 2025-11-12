import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { accountService } from '../services/accountService';

export default function Profile() {
  const { user, logout } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({
    accountName: '',
    accountEmail: '',
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });

  useEffect(() => {
    if (user) {
      setFormData(prev => ({
        ...prev,
        accountName: user.accountName || '',
        accountEmail: user.accountEmail || ''
      }));
    }
  }, [user]);

  const getRoleName = () => {
    const roles = { 0: 'Admin', 1: 'Staff', 2: 'Lecturer' };
    return roles[user?.accountRole] || 'Unknown';
  };

  const handleEditClick = () => {
    setFormData(prev => ({
      ...prev,
      accountName: user?.accountName || '',
      accountEmail: user?.accountEmail || '',
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    }));
    setError('');
    setSuccess('');
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validation
    if (!formData.accountName.trim()) {
      setError('Name cannot be empty');
      return;
    }

    if (!formData.accountEmail.trim()) {
      setError('Email cannot be empty');
      return;
    }

    if (formData.newPassword) {
      if (!formData.currentPassword) {
        setError('Current password is required to set new password');
        return;
      }
      if (formData.newPassword.length < 6) {
        setError('New password must be at least 6 characters');
        return;
      }
      if (formData.newPassword !== formData.confirmPassword) {
        setError('Passwords do not match');
        return;
      }
    }

    try {
      setLoading(true);
      setError('');
      setSuccess('');

      const updateData = {
        accountName: formData.accountName.trim(),
        accountEmail: formData.accountEmail.trim(),
        accountRole: user.accountRole,
        accountPassword: formData.newPassword || 'unchanged' // API expects password field
      };

      await accountService.update(user.accountId, updateData);

      setSuccess('Profile updated successfully');
      setShowModal(false);
      
      // Update local storage
      const updatedUser = { ...user, accountName: updateData.accountName, accountEmail: updateData.accountEmail };
      localStorage.setItem('user', JSON.stringify(updatedUser));
      
      // Reload page or re-authenticate
      setTimeout(() => {
        window.location.reload();
      }, 1500);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update profile');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    if (window.confirm('Are you sure you want to logout?')) {
      logout();
    }
  };

  return (
    <div>
      <div className="page-header">
        <h1>My Profile</h1>
        <p>Manage your account information</p>
      </div>

      {error && <div className="error-message">{error}</div>}
      {success && <div style={{ padding: '15px', background: '#d4edda', color: '#155724', borderRadius: '4px', marginBottom: '20px' }}>{success}</div>}

      <div className="card" style={{ maxWidth: '600px', margin: '20px auto' }}>
        <div style={{ marginBottom: '30px', paddingBottom: '20px', borderBottom: '1px solid #ddd' }}>
          <h2>Account Information</h2>
          
          <div style={{ marginTop: '20px' }}>
            <div style={{ marginBottom: '15px' }}>
              <label style={{ display: 'block', fontWeight: 'bold', marginBottom: '5px' }}>Name</label>
              <p style={{ margin: '10px 0', fontSize: '16px' }}>{user?.accountName}</p>
            </div>

            <div style={{ marginBottom: '15px' }}>
              <label style={{ display: 'block', fontWeight: 'bold', marginBottom: '5px' }}>Email</label>
              <p style={{ margin: '10px 0', fontSize: '16px' }}>{user?.accountEmail}</p>
            </div>

            <div style={{ marginBottom: '15px' }}>
              <label style={{ display: 'block', fontWeight: 'bold', marginBottom: '5px' }}>Role</label>
              <p style={{ margin: '10px 0', fontSize: '16px' }}>{getRoleName()}</p>
            </div>

            <div style={{ marginBottom: '15px' }}>
              <label style={{ display: 'block', fontWeight: 'bold', marginBottom: '5px' }}>Account ID</label>
              <p style={{ margin: '10px 0', fontSize: '16px' }}>{user?.accountId}</p>
            </div>
          </div>
        </div>

        <div style={{ display: 'flex', gap: '10px', justifyContent: 'center' }}>
          <button 
            onClick={handleEditClick}
            className="btn btn-primary"
          >
            Edit Profile
          </button>
          
          <button 
            onClick={handleLogout}
            className="btn btn-danger"
          >
            Logout
          </button>
        </div>
      </div>

      {/* Edit Modal */}
      {showModal && (
        <div className="modal-overlay" onClick={() => !loading && setShowModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()} style={{ maxWidth: '500px' }}>
            <h2>Edit Profile</h2>
            {error && <div className="error-message">{error}</div>}
            
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Name *</label>
                <input
                  type="text"
                  value={formData.accountName}
                  onChange={(e) => setFormData({ ...formData, accountName: e.target.value })}
                  required
                  disabled={loading}
                />
              </div>

              <div className="form-group">
                <label>Email *</label>
                <input
                  type="email"
                  value={formData.accountEmail}
                  onChange={(e) => setFormData({ ...formData, accountEmail: e.target.value })}
                  required
                  disabled={loading}
                />
              </div>

              <hr style={{ margin: '20px 0', borderTop: '1px solid #ddd' }} />

              <h3 style={{ fontSize: '16px', marginTop: '20px', marginBottom: '10px' }}>Change Password (Optional)</h3>

              <div className="form-group">
                <label>Current Password</label>
                <input
                  type="password"
                  value={formData.currentPassword}
                  onChange={(e) => setFormData({ ...formData, currentPassword: e.target.value })}
                  placeholder="Enter current password if changing password"
                  disabled={loading}
                />
              </div>

              <div className="form-group">
                <label>New Password</label>
                <input
                  type="password"
                  value={formData.newPassword}
                  onChange={(e) => setFormData({ ...formData, newPassword: e.target.value })}
                  placeholder="Leave empty to keep current password"
                  disabled={loading}
                />
              </div>

              <div className="form-group">
                <label>Confirm Password</label>
                <input
                  type="password"
                  value={formData.confirmPassword}
                  onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                  placeholder="Confirm new password"
                  disabled={loading}
                />
              </div>

              <div className="form-actions">
                <button 
                  type="button" 
                  onClick={() => setShowModal(false)}
                  className="btn"
                  disabled={loading}
                >
                  Cancel
                </button>
                <button 
                  type="submit" 
                  className="btn btn-primary"
                  disabled={loading}
                >
                  {loading ? 'Saving...' : 'Save Changes'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
