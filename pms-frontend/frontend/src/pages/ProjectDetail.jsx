import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import Layout from '../components/Layout.jsx';
import api from '../api/axios';

export default function ProjectDetail() {
  const { id } = useParams();
  const [project, setProject] = useState(null);
  const [tasks, setTasks] = useState([]);
  const [report, setReport] = useState(null);
  const [showTaskForm, setShowTaskForm] = useState(false);
  const [taskForm, setTaskForm] = useState({ title: '', description: '', priority: 'medium', assignedToId: '' });
  const [statusFilter, setStatusFilter] = useState('');
  const [expandedTask, setExpandedTask] = useState(null);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');

  async function loadAll() {
    const [projectRes, reportRes] = await Promise.all([
      api.get(`/projects/${id}`),
      api.get(`/reports/project/${id}`),
    ]);
    setProject(projectRes.data);
    setReport(reportRes.data);
    loadTasks();
  }

  async function loadTasks() {
    const params = {};
    if (statusFilter) params.status = statusFilter;
    const res = await api.get(`/projects/${id}/tasks`, { params });
    setTasks(res.data);
  }

  useEffect(() => {
    loadAll();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  useEffect(() => {
    loadTasks();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [statusFilter]);

  async function handleCreateTask(e) {
    e.preventDefault();
    await api.post(`/projects/${id}/tasks`, {
      title: taskForm.title,
      description: taskForm.description,
      priority: taskForm.priority,
      assignedToId: taskForm.assignedToId ? Number(taskForm.assignedToId) : null,
    });
    setTaskForm({ title: '', description: '', priority: 'medium', assignedToId: '' });
    setShowTaskForm(false);
    loadTasks();
  }

  async function updateTaskStatus(taskId, newStatus) {
    await api.put(`/projects/${id}/tasks/${taskId}`, { status: newStatus });
    loadTasks();
  }

  async function toggleComments(taskId) {
    if (expandedTask === taskId) {
      setExpandedTask(null);
      return;
    }
    const res = await api.get(`/tasks/${taskId}/comments`);
    setComments(res.data);
    setExpandedTask(taskId);
  }

  async function handleAddComment(taskId) {
    if (!newComment.trim()) return;
    await api.post(`/tasks/${taskId}/comments`, { content: newComment });
    setNewComment('');
    const res = await api.get(`/tasks/${taskId}/comments`);
    setComments(res.data);
  }

  if (!project) {
    return (
      <Layout>
        <p style={{ color: 'var(--muted)' }}>Loading project…</p>
      </Layout>
    );
  }

  return (
    <Layout>
      <Link to="/projects" style={{ fontSize: '0.85rem', color: 'var(--muted)' }}>← Back to projects</Link>
      <div className="main-header" style={{ marginTop: 8 }}>
        <div>
          <h1>{project.title}</h1>
          <p>{project.description}</p>
        </div>
        <button className="btn btn-primary" onClick={() => setShowTaskForm((s) => !s)}>
          {showTaskForm ? 'Cancel' : '+ New task'}
        </button>
      </div>

      {report && (
        <div className="stat-grid" style={{ gridTemplateColumns: 'repeat(4, 1fr)' }}>
          <div className="stat-card">
            <div className="num">{report.progressPercent}%</div>
            <div className="label">Progress</div>
          </div>
          <div className="stat-card">
            <div className="num">{report.totalTasks}</div>
            <div className="label">Total tasks</div>
          </div>
          <div className="stat-card">
            <div className="num">{report.completedTasks}</div>
            <div className="label">Completed</div>
          </div>
          <div className="stat-card">
            <div className="num">{report.todoTasks}</div>
            <div className="label">To do</div>
          </div>
        </div>
      )}

      {showTaskForm && (
        <div className="card" style={{ marginBottom: 20 }}>
          <form onSubmit={handleCreateTask}>
            <div className="field">
              <label>Title</label>
              <input required value={taskForm.title} onChange={(e) => setTaskForm({ ...taskForm, title: e.target.value })} />
            </div>
            <div className="field">
              <label>Description</label>
              <textarea rows={2} value={taskForm.description} onChange={(e) => setTaskForm({ ...taskForm, description: e.target.value })} />
            </div>
            <div style={{ display: 'flex', gap: 12 }}>
              <div className="field" style={{ flex: 1 }}>
                <label>Priority</label>
                <select value={taskForm.priority} onChange={(e) => setTaskForm({ ...taskForm, priority: e.target.value })}>
                  <option value="low">Low</option>
                  <option value="medium">Medium</option>
                  <option value="high">High</option>
                </select>
              </div>
              <div className="field" style={{ flex: 1 }}>
                <label>Assign to (User ID)</label>
                <input value={taskForm.assignedToId} onChange={(e) => setTaskForm({ ...taskForm, assignedToId: e.target.value })} placeholder="Optional" />
              </div>
            </div>
            <button type="submit" className="btn btn-primary">Create task</button>
          </form>
        </div>
      )}

      <div style={{ display: 'flex', gap: 10, marginBottom: 16 }}>
        {['', 'todo', 'in-progress', 'done'].map((s) => (
          <button
            key={s}
            className="btn btn-ghost"
            style={{ background: statusFilter === s ? 'var(--ink)' : undefined, color: statusFilter === s ? '#fff' : undefined }}
            onClick={() => setStatusFilter(s)}
          >
            {s === '' ? 'All' : s}
          </button>
        ))}
      </div>

      {tasks.length === 0 ? (
        <div className="empty-state">No tasks here yet — add the first one above.</div>
      ) : (
        tasks.map((t) => (
          <div key={t.id} style={{ marginBottom: 10 }}>
            <div className="task-row">
              <div>
                <div className="title">{t.title}</div>
                <div className="meta">{t.description}</div>
              </div>
              <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
                <span className={`badge ${t.priority}`}>{t.priority}</span>
                <select value={t.status} onChange={(e) => updateTaskStatus(t.id, e.target.value)} style={{ padding: '5px 8px', borderRadius: 6, border: '1px solid var(--line)' }}>
                  <option value="todo">To do</option>
                  <option value="in-progress">In progress</option>
                  <option value="done">Done</option>
                </select>
                <button className="btn btn-ghost" onClick={() => toggleComments(t.id)}>
                  Comments
                </button>
              </div>
            </div>

            {expandedTask === t.id && (
              <div className="card" style={{ marginTop: -4, marginBottom: 8 }}>
                {comments.length === 0 && <p style={{ color: 'var(--muted)', fontSize: '0.85rem' }}>No comments yet.</p>}
                {comments.map((c) => (
                  <div key={c.id} className="comment-item">
                    <div>{c.content}</div>
                    <div className="meta">{new Date(c.createdAt).toLocaleString()}</div>
                  </div>
                ))}
                <div style={{ display: 'flex', gap: 8, marginTop: 10 }}>
                  <input
                    placeholder="Write a comment…"
                    value={newComment}
                    onChange={(e) => setNewComment(e.target.value)}
                    style={{ flex: 1, padding: '8px 10px', border: '1px solid var(--line)', borderRadius: 8 }}
                  />
                  <button className="btn btn-primary" onClick={() => handleAddComment(t.id)}>Post</button>
                </div>
              </div>
            )}
          </div>
        ))
      )}
    </Layout>
  );
}
