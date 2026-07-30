import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';

export default function Layout({ children }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="dot" />
          Flowline
        </div>
        <nav>
          <NavLink to="/dashboard" className={({ isActive }) => (isActive ? 'active' : '')}>
            Dashboard
          </NavLink>
          <NavLink to="/projects" className={({ isActive }) => (isActive ? 'active' : '')}>
            Projects
          </NavLink>
          <NavLink to="/notifications" className={({ isActive }) => (isActive ? 'active' : '')}>
            Notifications
          </NavLink>
        </nav>
        <div className="user-box">
          <div style={{ color: '#e8e6df', fontWeight: 600, marginBottom: 2 }}>{user?.username || 'User'}</div>
          <div style={{ marginBottom: 10 }}>{user?.role || 'User'}</div>
          <button onClick={handleLogout} style={{ padding: 0, background: 'none', color: '#c7c9cf', fontSize: '0.85rem' }}>
            Log out
          </button>
        </div>
      </aside>
      <main className="main">{children}</main>
    </div>
  );
}
