# FU News Management System - Frontend

Ứng dụng React Vite đơn giản để quản lý tin tức FU.

## Cài đặt

```bash
npm install
```

## Cấu hình

Mở file `src/services/api.js` và thay đổi `API_URL` để trỏ đến backend API của bạn:

```javascript
const API_URL = 'http://localhost:5000/api'; // Thay đổi URL này
```

## Chạy ứng dụng

```bash
npm run dev
```

Ứng dụng sẽ chạy tại `http://localhost:5173`

## Cấu trúc dự án

```
src/
├── components/         # Reusable components
│   └── ProtectedRoute.jsx
├── contexts/          # React Context
│   └── AuthContext.jsx
├── pages/            # Pages/Routes
│   ├── Login.jsx
│   ├── Dashboard.jsx
│   ├── Home.jsx
│   ├── Accounts.jsx
│   ├── Categories.jsx
│   └── NewsArticles.jsx
├── services/         # API services
│   ├── api.js
│   ├── authService.js
│   ├── accountService.js
│   ├── categoryService.js
│   ├── newsArticleService.js
│   └── tagService.js
├── App.jsx           # Main app component
└── main.jsx         # Entry point
```

## Tính năng

### Authentication
- Login/Logout
- JWT token management
- Role-based access control

### Account Management (Admin only)
- View all accounts
- Create new account
- Edit account
- Delete account
- Support roles: Admin, Staff, Lecturer, Teacher, Student

### Category Management (Staff/Admin)
- View all categories
- Create new category
- Edit category
- Delete category (if no dependencies)
- Toggle category status
- Support parent-child categories

### News Article Management (Staff/Admin)
- View all news articles
- Create new article
- Edit own articles
- Delete own articles
- Add/remove tags
- Select category
- Set article status (Active/Inactive)

## Quyền truy cập

- **Admin (Role = 1)**: Truy cập tất cả tính năng
- **Staff (Role = 2)**: Quản lý Categories và News Articles
- **Others**: Chỉ xem dashboard

## Đăng nhập mẫu

Sử dụng tài khoản từ database của bạn. Ví dụ:
- Email: admin@example.com
- Password: (password trong database)

## Build cho production

```bash
npm run build
```

File build sẽ được tạo trong thư mục `dist/`
