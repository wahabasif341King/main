import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Layout from '../components/Layout.jsx';
import api from '../api/axios';

export default function Projects() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ title: '', description: '', dueDate: '' });
  const [error, setError] = useState('');

  async function loadProjects() {
    setLoading(true);
    try {
      const params = {};
      if (search) params.search = search;
      if (status) params.status = status;
      const res = await api.get('/projects', { params });
      setProjects(res.data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadProjects();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [status]);

  function handleSearchSubmit(e) {
    e.preventDefault();
    loadProjects();
  }

  async function handleCreate(e) {
    e.preventDefault();
    setError('');
    try {
      await api.post('/projects', {
        title: form.title,
        description: form.description,
        dueDate: form.dueDate || null,
      });
      setForm({ title: '', description: '', dueDate: '' });
      setShowForm(false);
      loadProjects();
    } catch (err) {
      setError('Could not create project. Check the fields and try again.');
    }
  }

  return (
    <Layout>
      <div className="main-header">
        <div>
          <h1>Projects</h1>
          <p>Everything your team is currently working across.</p>
        </div>
        <button className="btn btn-primary" onClick={() => setShowForm((s) => !s)}>
          {showForm ? 'Cancel' : '+ New project'}
        </button>
      </div>

      {showForm && (
        <div className="card" style={{ marginBottom: 24 }}>
          {error && <div className="error-msg">{error}</div>}
          <form onSubmit={handleCreate}>
            <div className="field">
              <label>Title</label>
              <input required value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} />
            </div>
            <div className="field">
              <label>Description</label>
              <textarea rows={2} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
            </div>
            <div className="field">
              <label>Due date</label>
              <input type="date" value={form.dueDate} onChange={(e) => setForm({ ...form, dueDate: e.target.value })} />
            </div>
            <button type="submit" className="btn btn-primary">Create project</button>
          </form>
        </div>
      )}

      <form onSubmit={handleSearchSubmit} style={{ display: 'flex', gap: 10, marginBottom: 20 }}>
        <input
          placeholder="Search projects…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          style={{ flex: 1, padding: '9px 12px', border: '1px solid var(--line)', borderRadius: 8 }}
        />
        <select value={status} onChange={(e) => setStatus(e.target.value)} style={{ padding: '9px 12px', border: '1px solid var(--line)', borderRadius: 8 }}>
          <option value="">All statuses</option>
          <option value="active">Active</option>
          <option value="completed">Completed</option>
          <option value="archived">Archived</option>
        </select>
        <button type="submit" className="btn btn-ghost">Search</button>
      </form>

      {loading ? (
        <p style={{ color: 'var(--muted)' }}>Loading…</p>
      ) : projects.length === 0 ? (
        <div className="empty-state">No projects match yet — create one to get started.</div>
      ) : (
        <div className="project-grid">
          {projects.map((p) => (
            <Link key={p.id} to={`/projects/${p.id}`} className="project-card">
              <span className={`badge ${p.status}`} style={{ marginBottom: 8, display: 'inline-block' }}>{p.status}</span>
              <h3>{p.title}</h3>
              <p>{p.description || 'No description yet.'}</p>
              {p.dueDate && <div style={{ fontSize: '0.78rem', color: 'var(--muted)' }}>Due {new Date(p.dueDate).toLocaleDateString()}</div>}
            </Link>
          ))}
        </div>
      )}
    </Layout>
  );
}
