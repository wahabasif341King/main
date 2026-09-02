import axiosInstance from './axiosInstance.js';

// ---- Warehouses ----
export const getWarehouses = () => axiosInstance.get('/warehouse');
export const createWarehouse = (data) => axiosInstance.post('/warehouse', data);
export const updateWarehouse = (id, data) => axiosInstance.put(`/warehouse/${id}`, data);
export const deleteWarehouse = (id) => axiosInstance.delete(`/warehouse/${id}`);

// ---- Inventory ----
export const getInventory = (warehouseId) =>
  axiosInstance.get('/inventory', { params: warehouseId ? { warehouseId } : {} });
export const adjustStock = (data) => axiosInstance.post('/inventory/adjust', data);

// ---- Stock Transfers ----
export const getStockTransfers = () => axiosInstance.get('/stocktransfer');
export const createStockTransfer = (data) => axiosInstance.post('/stocktransfer', data);
export const updateStockTransferStatus = (id, newStatus) =>
  axiosInstance.patch(`/stocktransfer/${id}/status`, JSON.stringify(newStatus), {
    headers: { 'Content-Type': 'application/json' },
  });