import { createContext, useContext, useState } from 'react';
import api from '../api/axios';
import { decodeToken } from '../api/jwt';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [token, setToken] = useState(localStorage.getItem('pms_token'));
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem('pms_token');
    return saved ? decodeToken(saved) : null;
  });

  async function login(email_Adress, password) {
    const res = await api.post('/auth/login', { email_Adress, password });
    const newToken = res.data.token;
    localStorage.setItem('pms_token', newToken);
    setToken(newToken);
    setUser(decodeToken(newToken));
    return res.data;
  }

  async function register(username, email_Address, password) {
    return api.post('/auth/register', { username, email_Address, password });
  }

  function logout() {
    localStorage.removeItem('pms_token');
    setToken(null);
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, token, login, register, logout, isAuthenticated: !!token }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
