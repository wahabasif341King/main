import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(form.email, form.password);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data || 'Invalid email or password.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="brand" style={{ display: 'flex', gap: 8, alignItems: 'center', marginBottom: 18 }}>
          <span style={{ width: 10, height: 10, borderRadius: '50%', background: 'var(--teal)', display: 'inline-block' }} />
          <strong style={{ fontFamily: "'Space Grotesk', sans-serif" }}>Flowline</strong>
        </div>
        <h1>Welcome back</h1>
        <p className="sub">Log in to see what's moving across your projects.</p>

        {error && <div className="error-msg">{typeof error === 'string' ? error : 'Something went wrong.'}</div>}

        <form onSubmit={handleSubmit}>
          <div className="field">
            <label>Email</label>
            <input
              type="email"
              required
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              placeholder="you@example.com"
            />
          </div>
          <div className="field">
            <label>Password</label>
            <input
              type="password"
              required
              value={form.password}
              onChange={(e) => setForm({ ...form, password: e.target.value })}
              placeholder="••••••••"
            />
          </div>
          <button type="submit" className="btn btn-primary" style={{ width: '100%', marginTop: 8 }} disabled={loading}>
            {loading ? 'Logging in…' : 'Log in'}
          </button>
        </form>

        <p style={{ fontSize: '0.85rem', color: 'var(--muted)', marginTop: 18, textAlign: 'center' }}>
          No account yet? <Link to="/register" style={{ color: 'var(--teal-dark)', fontWeight: 600 }}>Register</Link>
        </p>
      </div>
    </div>
  );
}
