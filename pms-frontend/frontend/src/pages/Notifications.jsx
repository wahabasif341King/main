import { useEffect, useState } from 'react';
import Layout from '../components/Layout.jsx';
import api from '../api/axios';

export default function Notifications() {
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    const res = await api.get('/notifications');
    setNotifications(res.data);
    setLoading(false);
  }

  useEffect(() => {
    load();
  }, []);

  async function markRead(id) {
    await api.put(`/notifications/${id}/read`);
    load();
  }

  async function markAllRead() {
    await api.put('/notifications/read-all');
    load();
  }

  return (
    <Layout>
      <div className="main-header">
        <div>
          <h1>Notifications</h1>
          <p>Task assignments and updates land here.</p>
        </div>
        <button className="btn btn-ghost" onClick={markAllRead}>Mark all as read</button>
      </div>

      {loading ? (
        <p style={{ color: 'var(--muted)' }}>Loading…</p>
      ) : notifications.length === 0 ? (
        <div className="empty-state">You're all caught up.</div>
      ) : (
        notifications.map((n) => (
          <div key={n.id} className={`notif-item ${n.isRead ? 'read' : 'unread'}`} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <div>{n.message}</div>
              <div style={{ fontSize: '0.75rem', opacity: 0.7 }}>{new Date(n.createdAt).toLocaleString()}</div>
            </div>
            {!n.isRead && (
              <button className="btn btn-ghost" onClick={() => markRead(n.id)}>Mark read</button>
            )}
          </div>
        ))
      )}
    </Layout>
  );
}
