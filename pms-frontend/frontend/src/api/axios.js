import axios from 'axios';

// Backend ka base URL — apna port yahan match karo (dotnet run se jo dikhe)
const api = axios.create({
  baseURL: 'http://localhost:5118/api',
});

// Har request ke saath Bearer token automatically attach karo
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('pms_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Agar token expire/invalid ho (401), user ko login pe bhej do
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('pms_token');
      localStorage.removeItem('pms_user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
