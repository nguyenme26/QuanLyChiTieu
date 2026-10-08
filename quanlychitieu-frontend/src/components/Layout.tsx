import React from 'react';
import { Link, useLocation, NavLink } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useTheme } from '../contexts/ThemeContext';

const navItems = [
  { path: '/dashboard', label: 'Tổng quan', icon: '📊' },
  { path: '/transactions', label: 'Giao dịch', icon: '💳' },
  { path: '/wallets', label: 'Ví', icon: '👛' },
  { path: '/categories', label: 'Danh mục', icon: '📁' },
  { path: '/budgets', label: 'Ngân sách', icon: '🎯' },
  { path: '/reports', label: 'Báo cáo', icon: '📈' },
];

export function Sidebar() {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const location = useLocation();
  const [isCollapsed, setIsCollapsed] = React.useState(false);

  return (
    <aside className={`sidebar ${isCollapsed ? 'collapsed' : ''}`} data-theme={theme}>
      <div className="sidebar-header">
        {!isCollapsed && (
          <Link to="/dashboard" className="logo">
            <span className="logo-icon">💰</span>
            <span className="logo-text">QuanLyChiTieu</span>
          </Link>
        )}
        <button 
          className="btn btn-ghost btn-icon sidebar-toggle"
          onClick={() => setIsCollapsed(!isCollapsed)}
          aria-label={isCollapsed ? 'Mở rộng' : 'Thu gọn'}
        >
          {isCollapsed ? '⟶' : '⟵'}
        </button>
      </div>

      <nav className="sidebar-nav">
        <ul>
          {navItems.map((item) => {
            const isActive = location.pathname === item.path;
            return (
              <li key={item.path}>
                <NavLink
                  to={item.path}
                  className={`nav-link ${isActive ? 'active' : ''}`}
                  onClick={() => setIsCollapsed(true)}
                >
                  <span className="nav-icon" aria-hidden="true">{item.icon}</span>
                  {!isCollapsed && <span className="nav-label">{item.label}</span>}
                  {isActive && !isCollapsed && <span className="nav-indicator" />}
                </NavLink>
              </li>
            );
          })}
        </ul>
      </nav>

      <div className="sidebar-footer">
        {!isCollapsed && user && (
          <div className="user-info">
            <div className="avatar" style={{ background: 'var(--gradient-primary)' }}>
              {user.fullName.charAt(0).toUpperCase()}
            </div>
            <div className="user-details">
              <span className="user-name">{user.fullName}</span>
              <span className="user-email">{user.email}</span>
            </div>
          </div>
        )}
        
        <div className="sidebar-actions">
          <button 
            className="btn btn-ghost btn-icon"
            onClick={toggleTheme}
            aria-label={theme === 'dark' ? 'Chế độ sáng' : 'Chế độ tối'}
            data-tooltip={theme === 'dark' ? 'Chế độ sáng' : 'Chế độ tối'}
            style={{ display: isCollapsed ? 'flex' : 'none' }}
          >
            {theme === 'dark' ? '☀️' : '🌙'}
          </button>
          {!isCollapsed && user && (
            <button className="btn btn-ghost btn-sm" onClick={logout} style={{ width: '100%' }}>
              Đăng xuất
            </button>
          )}
        </div>
      </div>
    </aside>
  );
}

export function Header() {
  const { theme, toggleTheme } = useTheme();
  const { user, isAuthenticated } = useAuth();

  return (
    <header className="header">
      <div className="header-left">
        <h1 className="page-title">Tổng quan</h1>
      </div>
      <div className="header-right">
        <button 
          className="btn btn-ghost btn-icon"
          onClick={toggleTheme}
          aria-label={theme === 'dark' ? 'Chế độ sáng' : 'Chế độ tối'}
        >
          {theme === 'dark' ? '☀️' : '🌙'}
        </button>
        {isAuthenticated && user && (
          <div className="header-user">
            <div className="avatar" style={{ background: 'var(--gradient-primary)' }}>
              {user.fullName.charAt(0).toUpperCase()}
            </div>
          </div>
        )}
      </div>
    </header>
  );
}

export function Layout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="loading-screen">
        <div className="spinner spinner-lg"></div>
      </div>
    );
  }

  return (
    <div className="layout">
      {isAuthenticated && <Sidebar />}
      <div className="main-wrapper">
        {isAuthenticated && <Header />}
        <main className="main-content">
          <div className="container">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
}
