import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export const ProtectedRoute = ({ children, requireAdmin = false, requireStaff = false, requireDashboard = false }) => {
  const { isAuthenticated, isAdmin, isStaff, loading } = useAuth();

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // Dashboard access: only admin and staff
  if (requireDashboard && !(isAdmin() || isStaff())) {
    return <Navigate to="/news" replace />;
  }

  if (requireAdmin && !isAdmin()) {
    return <Navigate to="/news" replace />;
  }

  if (requireStaff && !(isStaff() || isAdmin())) {
    return <Navigate to="/news" replace />;
  }

  return children;
};
