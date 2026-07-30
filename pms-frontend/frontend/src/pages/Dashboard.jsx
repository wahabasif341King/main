import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Layout from '../components/Layout.jsx';
import api from '../api/axios';

export default function Dashboard() {
  const [summary, setSummary] = useState(null);
  const [recent, setRecent] = useState({ recentProjects: [], recentTasks: [] });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const [summaryRes, recentRes] = await Promise.all([
          api.get('/dashboard/summary'),
          api.get('/dashboard/recent'),
        ]);
        setSummary(summaryRes.data);
        setRecent(recentRes.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  return (
    <Layout>
      <div className="main-header">
        <div>
          <h1>Dashboard</h1>
          <p>A quick read on where things stand today.</p>
        </div>
        <Link to="/projects" className="btn btn-primary">+ New project</Link>
      </div>

      {loading ? (
        <p style={{ color: 'var(--muted)' }}>Loading…</p>
      ) : (
        <>
          <div className="stat-grid">
            <div className="stat-card">
              <div className="num">{summary?.totalProjects ?? 0}</div>
              <div className="label">Total projects</div>
            </div>
            <div className="stat-card">
              <div className="num">{summary?.totalTasks ?? 0}</div>
              <div className="label">Total tasks</div>
            </div>
            <div className="stat-card">
              <div className="num">{summary?.tasksInProgress ?? 0}</div>
              <div className="label">In progress</div>
            </div>
            <div className="stat-card">
              <div className="num" style={{ color: (summary?.overdueTasks ?? 0) > 0 ? 'var(--coral)' : 'var(--ink)' }}>
                {summary?.overdueTasks ?? 0}
              </div>
              <div className="label">Overdue tasks</div>
            </div>
          </div>

          <div className="two-col">
            <div>
              <div className="section-title">Recent projects</div>
              {recent.recentProjects.length === 0 && <p style={{ color: 'var(--muted)' }}>No projects yet.</p>}
              {recent.recentProjects.map((p) => (
                <Link key={p.id} to={`/projects/${p.id}`} className="card" style={{ display: 'block', marginBottom: 10 }}>
                  <strong>{p.title}</strong>
                  <div style={{ fontSize: '0.8rem', color: 'var(--muted)', marginTop: 4 }}>
                    <span className={`badge ${p.status}`}>{p.status}</span>
                  </div>
                </Link>
              ))}
            </div>
            <div>
              <div className="section-title">Recent tasks</div>
              {recent.recentTasks.length === 0 && <p style={{ color: 'var(--muted)' }}>No tasks yet.</p>}
              {recent.recentTasks.map((t) => (
                <div key={t.id} className="task-row">
                  <div>
                    <div className="title">{t.title}</div>
                    <div className="meta">{t.status}</div>
                  </div>
                  <span className={`badge ${t.priority}`}>{t.priority}</span>
                </div>
              ))}
            </div>
          </div>
        </>
      )}
    </Layout>
  );
}
