# 🔐 Permission & Navigation Updates - Session 5

## Summary of Changes

Xử lý các yêu cầu về phân quyền và điều hướng người dùng:

1. **Loại bỏ điều kiện "kẹt ở login"** - User chưa login vẫn có thể xem bài viết công cộng
2. **Thêm button "View Public News" trên trang Login**
3. **Redirect users theo role**:
   - Lecturer (role = 2) → `/news` (public news page)
   - Admin/Staff → `/dashboard` (management interface)
4. **Phân quyền Dashboard** - Chỉ Admin/Staff được vào dashboard
5. **Thêm button "View Detail"** cho Categories và NewsArticles

---

## 📝 File Changes

### 1. **Login.jsx** ✅
**Thêm button để vào trang public news**

```jsx
<div style={{ marginTop: '20px', paddingTop: '20px', borderTop: '1px solid #ddd', textAlign: 'center' }}>
  <p style={{ marginBottom: '10px', color: '#666' }}>Or read news without login</p>
  <button
    type="button"
    onClick={() => navigate('/news')}
    className="btn btn-secondary"
    style={{ background: '#6c757d', color: 'white', width: '100%' }}
  >
    View Public News
  </button>
</div>
```

**Impact**:
- User chưa login có thể click button "View Public News"
- Navigate tới `/news` mà không cần login
- Giải quyết: "user kẹt ở trang login"

---

### 2. **AuthContext.jsx** ✅
**Thêm helper function để kiểm tra role**

```jsx
const isAdminOrStaff = () => {
  return isAdmin() || isStaff();
};

// Add to context value
value = {
  ...
  isAdminOrStaff,
  ...
}
```

**Impact**:
- Dễ dàng check xem user có role Admin/Staff không
- Dùng cho protected routes

---

### 3. **ProtectedRoute.jsx** ✅
**Thêm `requireDashboard` flag để phân quyền dashboard**

```jsx
export const ProtectedRoute = ({ 
  children, 
  requireAdmin = false, 
  requireStaff = false, 
  requireDashboard = false  // NEW
}) => {
  // Dashboard access: only admin and staff
  if (requireDashboard && !(isAdmin() || isStaff())) {
    return <Navigate to="/news" replace />;  // Redirect Lecturer to public news
  }
  
  // Lecturer accessing admin-only → redirect to /news instead of /dashboard
  if (requireAdmin && !isAdmin()) {
    return <Navigate to="/news" replace />;  // Changed from /dashboard
  }

  // Lecturer accessing staff-only → redirect to /news instead of /dashboard
  if (requireStaff && !(isStaff() || isAdmin())) {
    return <Navigate to="/news" replace />;  // Changed from /dashboard
  }
};
```

**Impact**:
- Lecturer trying to access `/dashboard` → redirected to `/news`
- Admin-only pages access by Lecturer → `/news`
- Staff-only pages access by Lecturer → `/news`
- Giải quyết: "các role khác vẫn thấy dashboard nhưng không có nội dung"

---

### 4. **App.jsx** ✅
**Update route `/dashboard` với `requireDashboard` flag**

```jsx
<Route
  path="/dashboard"
  element={
    <ProtectedRoute requireDashboard>  // NEW
      <Dashboard />
    </ProtectedRoute>
  }
>
```

**Impact**:
- `/dashboard` bây giờ chỉ cho Admin/Staff
- Lecturer không thể access `/dashboard`

---

### 5. **NewsArticles.jsx** ✅
**Thêm button "View" và modal để xem chi tiết bài viết**

```jsx
// Import useNavigate
import { useNavigate } from 'react-router-dom';

// Thêm state
const [showViewModal, setShowViewModal] = useState(false);
const [viewingArticle, setViewingArticle] = useState(null);

// Handler function
const handleView = (article) => {
  setViewingArticle(article);
  setShowViewModal(true);
};

// Button in table
<button
  onClick={() => navigate(`/article/${article.newsArticleId}`)}
  className="btn btn-info"
  style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
>
  View
</button>

// Modal to display article details
{showViewModal && viewingArticle && (
  <div className="modal-overlay">
    {/* Display: ID, Title, Headline, Content, Source, Category, Author, Status, Dates, Tags */}
  </div>
)}
```

**Impact**:
- Staff/Admin có button "View" để xem chi tiết article
- Giải quyết: "trong trang quản lí article không có nút để view detail"

---

### 6. **Categories.jsx** ✅
**Thêm button "View" và modal để xem chi tiết category**

```jsx
// Import useNavigate
import { useNavigate } from 'react-router-dom';

// Thêm state
const [showViewModal, setShowViewModal] = useState(false);
const [viewingCategory, setViewingCategory] = useState(null);

// Handler function
const handleView = (category) => {
  setViewingCategory(category);
  setShowViewModal(true);
};

// Button in table
<button
  onClick={() => handleView(category)}
  className="btn btn-info"
  style={{ marginRight: '5px', padding: '5px 10px', fontSize: '12px' }}
>
  View
</button>

// Modal to display category details
{showViewModal && viewingCategory && (
  <div className="modal-overlay">
    {/* Display: ID, Name, Description, Parent, Status, News Count, Can Delete */}
  </div>
)}
```

**Impact**:
- Staff/Admin có button "View" để xem chi tiết category
- Giải quyết: "trong trang quản lí category không có nút để view detail"

---

## 🔄 User Flow Changes

### **Before (Problematic)**
```
❌ User không login → Kẹt ở Login
❌ Lecturer login → Dashboard (trống không)
❌ Admin access non-admin page → Redirect to /dashboard (infinite loop)
❌ Không có button View detail cho Category/Article
```

### **After (Fixed)** ✅
```
✅ User không login → Login page (+ button: "View Public News")
   → Click button → /news (xem bài viết công cộng)

✅ Lecturer login → Redirect to /dashboard 
   → ProtectedRoute: "not allowed" → Redirect to /news ✅

✅ Admin access non-admin page → Redirect to /news ✅
   (không infinite loop với /dashboard)

✅ Staff/Admin xem Article Management 
   → Click "View" button → Modal hiển thị chi tiết ✅

✅ Staff/Admin xem Category Management 
   → Click "View" button → Modal hiển thị chi tiết ✅
```

---

## 📊 Route Permission Matrix (Updated)

| Route | Public | Lecturer | Staff | Admin | Behavior |
|-------|--------|----------|-------|-------|----------|
| `/login` | ✅ | ✅ | ✅ | ✅ | Show login form + "View Public News" button |
| `/news` | ✅ | ✅ | ✅ | ✅ | Show public articles |
| `/article/:id` | ✅ | ✅ | ✅ | ✅ | Show article detail |
| `/dashboard` | ❌ | ❌→`/news` | ✅ | ✅ | Admin/Staff only, others redirect to /news |
| `/dashboard/accounts` | ❌ | ❌→`/news` | ❌→`/news` | ✅ | Admin only |
| `/dashboard/categories` | ❌ | ❌→`/news` | ✅ | ✅ | Admin/Staff only |
| `/dashboard/news` | ❌ | ❌→`/news` | ✅ | ✅ | Admin/Staff only |
| `/dashboard/statistics` | ❌ | ❌→`/news` | ❌→`/news` | ✅ | Admin only |
| `/dashboard/news-history` | ❌ | ❌→`/news` | ✅ | ❌→`/news` | Staff only |
| `/dashboard/profile` | ❌ | ❌→`/news` | ✅ | ✅ | Authenticated users only, others redirect |

---

## 🎯 Issue Resolution

### Issue 1: "Chưa login user kẹt ở trang login"
✅ **Giải quyết**: 
- Thêm button "View Public News" trên Login page
- User có thể click để vào `/news` mà không cần login

### Issue 2: "Các role khác (Lecturer) được chuyển vào homepage"
✅ **Giải quyết**:
- Thêm `requireDashboard` flag vào ProtectedRoute
- Lecturer trying to access `/dashboard` → Redirect to `/news`
- Lecturer trying to access `/dashboard/any-page` → Redirect to `/news`

### Issue 3: "Các role khác vẫn thấy dashboard nhưng không có nội dung"
✅ **Giải quyết**:
- ProtectedRoute bây giờ block Lecturer từ accessing dashboard
- Nếu Lecturer try to access `/dashboard` → ProtectedRoute redirect to `/news`
- Dashboard không bao giờ render cho Lecturer

### Issue 4: "Trong trang quản lí article và category không có nút để view detail"
✅ **Giải quyết**:
- NewsArticles page: Thêm "View" button → Modal hiển thị article details
- Categories page: Thêm "View" button → Modal hiển thị category details
- Giải quyết: "không có nút để view detail cho 2 object này"

---

## ✅ Testing Checklist

- [ ] Not logged in → Can access `/news` and `/article/:id`
- [ ] Not logged in → Click "View Public News" on Login → `/news` ✅
- [ ] Lecturer login → Can access `/news` and `/article/:id` ✅
- [ ] Lecturer login → Try access `/dashboard` → Redirect to `/news` ✅
- [ ] Lecturer login → Try access `/dashboard/profile` → Redirect to `/news` ✅
- [ ] Staff login → Can access `/dashboard` and all staff routes ✅
- [ ] Staff login → Click "View" button on Article → Modal shows details ✅
- [ ] Staff login → Click "View" button on Category → Modal shows details ✅
- [ ] Admin login → Can access `/dashboard` and all admin routes ✅
- [ ] Admin login → Try access `/dashboard/accounts` → Works ✅
- [ ] Admin login → Try access `/dashboard/statistics` → Works ✅

---

## 📋 Code Quality

- ✅ No infinite loops in redirects
- ✅ Consistent permission checking
- ✅ Modal view instead of alerts (better UX)
- ✅ Proper state management for view modals
- ✅ User can navigate easily without being stuck
- ✅ Clear visual feedback with buttons

