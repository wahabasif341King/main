import axiosInstance from './axiosInstance.js';

export const registerOrganization = (data) => {
  return axiosInstance.post('/auth/register', data);
};

export const loginUser = (data) => {
  return axiosInstance.post('/auth/login', data);
};

export const registerEmployee = (data) => {
  return axiosInstance.post('/auth/register-employee', data);
};

export const getMe = () => {
  return axiosInstance.get('/auth/me');
};