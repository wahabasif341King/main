import axiosInstance from './axiosInstance.js';

// ---- Categories ----
export const getCategories = () => axiosInstance.get('/category');
export const createCategory = (data) => axiosInstance.post('/category', data);
export const updateCategory = (id, data) => axiosInstance.put(`/category/${id}`, data);
export const deleteCategory = (id) => axiosInstance.delete(`/category/${id}`);

// ---- Brands ----
export const getBrands = () => axiosInstance.get('/brand');
export const createBrand = (data) => axiosInstance.post('/brand', data);
export const updateBrand = (id, data) => axiosInstance.put(`/brand/${id}`, data);
export const deleteBrand = (id) => axiosInstance.delete(`/brand/${id}`);

// ---- Taxes ----
export const getTaxes = () => axiosInstance.get('/tax');
export const createTax = (data) => axiosInstance.post('/tax', data);
export const updateTax = (id, data) => axiosInstance.put(`/tax/${id}`, data);
export const deleteTax = (id) => axiosInstance.delete(`/tax/${id}`);

// ---- Products ----
export const getProducts = () => axiosInstance.get('/product');
export const getProductById = (id) => axiosInstance.get(`/product/${id}`);
export const createProduct = (data) => axiosInstance.post('/product', data);
export const updateProduct = (id, data) => axiosInstance.put(`/product/${id}`, data);
export const deleteProduct = (id) => axiosInstance.delete(`/product/${id}`);

// ---- Product Variants ----
export const getVariantsByProduct = (productId) =>
  axiosInstance.get(`/productvariant/by-product/${productId}`);
export const createProductVariant = (data) => axiosInstance.post('/productvariant', data);
export const updateProductVariant = (id, data) => axiosInstance.put(`/productvariant/${id}`, data);
export const deleteProductVariant = (id) => axiosInstance.delete(`/productvariant/${id}`);