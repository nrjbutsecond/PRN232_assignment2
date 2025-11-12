import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Home from './pages/Home';
import Accounts from './pages/Accounts';
import Categories from './pages/Categories';
import NewsArticles from './pages/NewsArticles';
import ArticleDetail from './pages/ArticleDetail';
import PublicNews from './pages/PublicNews';
import Profile from './pages/Profile';
import Statistics from './pages/Statistics';
import NewsHistory from './pages/NewsHistory';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/article/:id" element={<ArticleDetail />} />
          <Route path="/news" element={<PublicNews />} />
          
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute requireDashboard>
                <Dashboard />
              </ProtectedRoute>
            }
          >
            <Route index element={<Home />} />
            
            <Route
              path="accounts"
              element={
                <ProtectedRoute requireAdmin>
                  <Accounts />
                </ProtectedRoute>
              }
            />
            
            <Route
              path="categories"
              element={
                <ProtectedRoute requireStaff>
                  <Categories />
                </ProtectedRoute>
              }
            />
            
            <Route
              path="news"
              element={
                <ProtectedRoute requireStaff>
                  <NewsArticles />
                </ProtectedRoute>
              }
            />

            <Route
              path="profile"
              element={
                <ProtectedRoute>
                  <Profile />
                </ProtectedRoute>
              }
            />

            <Route
              path="statistics"
              element={
                <ProtectedRoute requireAdmin>
                  <Statistics />
                </ProtectedRoute>
              }
            />

            <Route
              path="news-history"
              element={
                <ProtectedRoute requireStaff>
                  <NewsHistory />
                </ProtectedRoute>
              }
            />
          </Route>
          
          <Route path="/" element={<Navigate to="/news" replace />} />
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
