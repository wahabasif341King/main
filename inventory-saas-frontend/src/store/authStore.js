import { create } from 'zustand';

const useAuthStore = create((set) => ({
  // Agar pehle se login hai (page refresh ke baad bhi), localStorage se load karo
  user: JSON.parse(localStorage.getItem('user')) || null,
  accessToken: localStorage.getItem('accessToken') || null,
  isAuthenticated: !!localStorage.getItem('accessToken'),

  // Login/Register success hone pe ye call hoga
  loginSuccess: (authResponse) => {
    const { accessToken, refreshToken, ...userData } = authResponse;

    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('user', JSON.stringify(userData));

    set({
      user: userData,
      accessToken: accessToken,
      isAuthenticated: true,
    });
  },

  // Logout
  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');

    set({
      user: null,
      accessToken: null,
      isAuthenticated: false,
    });
  },
}));

export default useAuthStore;