import { useState, useEffect } from 'react';
import { accountService } from '../services/accountService';
import { useAuth } from '../contexts/AuthContext';

export default function Accounts() {
  const { isAdmin } = useAuth();
  const [accounts, setAccounts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingAccount, setEditingAccount] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [formData, setFormData] = useState({
    accountName: '',
    accountEmail: '',
    accountPassword: '',
    accountRole: 2,
  });

  useEffect(() => {
    loadAccounts();
  }, []);

  const loadAccounts = async () => {
    try {
      setLoading(true);
      const data = await accountService.getAll();
      setAccounts(data);
      setError('');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load accounts');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingAccount(null);
    setFormData({
      accountName: '',
      accountEmail: '',
      accountPassword: '',
      accountRole: 2,
    });
    setShowModal(true);
  };

  const handleEdit = (account) => {
    setEditingAccount(account);
    setFormData({
      accountName: account.accountName,
      accountEmail: account.accountEmail,
      accountPassword: '',
      accountRole: account.accountRole,
    });
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingAccount) {
        await accountService.update(editingAccount.accountId, formData);
      } else {
        await accountService.create(formData);
      }
      setShowModal(false);
      loadAccounts();
    } catch (err) {
      setError(err.response?.data?.message || 'Operation failed');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this account?')) {
      try {
        await accountService.delete(id);
        loadAccounts();
      } catch (err) {
        setError(err.response?.data?.message || 'Failed to delete account');
      }
    }
  };

  const getRoleName = (role) => {
    // Backend: 0=Admin, 1=Staff, 2=Lecturer
    const roles = { 0: 'Admin', 1: 'Staff', 2: 'Lecturer' };
    return roles[role] || `Role ${role}`;
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      loadAccounts();
      return;
    }
    try {
      setLoading(true);
      const data = await accountService.search(searchTerm);
      // API returns direct array or wrapped in data property
      const accountsData = Array.isArray(data) ? data : (data?.data ? data.data : []);
      setAccounts(accountsData);
      setError('');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to search accounts');
      setAccounts([]);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div>
      <div className="page-header">
        <h1>Account Management</h1>
        <p>Manage system accounts</p>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="card">
        <div className="button-group">
          <form onSubmit={handleSearch} style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
            <input
              type="text"
              placeholder="Search by name or email..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              style={{ flex: 1, padding: '8px', border: '1px solid #ddd', borderRadius: '4px' }}
            />
            <button type="submit" className="btn btn-primary">Search</button>
            <button 
              type="button" 
              onClick={() => {
                setSearchTerm('');
                loadAccounts();
              }} 
              className="btn"
              style={{ background: '#6c757d', color: 'white' }}
            >
              Reset
            </button>
          </form>
          <button onClick={handleCreate} className="btn btn-primary">
            + Create Account
          </button>
        </div>

        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {accounts.map((account) => (
                <tr key={account.accountId}>
                  <td>{account.accountId}</td>
                  <td>{account.accountName}</td>
                  <td>{account.accountEmail}</td>
                  <td>{getRoleName(account.accountRole)}</td>
                  <td>
                    {/* Hide actions for Admin role (role 0) */}
                    {account.accountRole !== 0 && (
                      <>
                        <button
                          onClick={() => handleEdit(account)}
                          className="btn btn-warning"
                          style={{ marginRight: '10px', padding: '5px 10px', fontSize: '12px' }}
                        >
                          Edit
                        </button>
                        <button
                          onClick={() => handleDelete(account.accountId)}
                          className="btn btn-danger"
                          style={{ padding: '5px 10px', fontSize: '12px' }}
                        >
                          Delete
                        </button>
                      </>
                    )}
                    {account.accountRole === 0 && (
                      <span style={{ color: '#999', fontSize: '12px' }}>Protected</span>
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
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h2>{editingAccount ? 'Edit Account' : 'Create Account'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Name *</label>
                <input
                  type="text"
                  value={formData.accountName}
                  onChange={(e) => setFormData({ ...formData, accountName: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Email *</label>
                <input
                  type="email"
                  value={formData.accountEmail}
                  onChange={(e) => setFormData({ ...formData, accountEmail: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Password {!editingAccount && '*'}</label>
                <input
                  type="password"
                  value={formData.accountPassword}
                  onChange={(e) => setFormData({ ...formData, accountPassword: e.target.value })}
                  required={!editingAccount}
                />
              </div>

              <div className="form-group">
                <label>Role *</label>
                <select
                  value={formData.accountRole}
                  onChange={(e) => setFormData({ ...formData, accountRole: parseInt(e.target.value) })}
                  required
                >
                  <option value={0}>Admin</option>
                  <option value={1}>Staff</option>
                  <option value={2}>Lecturer</option>
                </select>
              </div>

              <div className="form-actions">
                <button type="button" onClick={() => setShowModal(false)} className="btn">
                  Cancel
                </button>
                <button type="submit" className="btn btn-primary">
                  {editingAccount ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
