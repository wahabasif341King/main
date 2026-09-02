import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Tune } from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import { getInventory, adjustStock, getWarehouses } from '../api/inventoryApi.js';
import { getProducts } from '../api/catalogApi.js';

function Inventory() {
  const [inventory, setInventory] = useState([]);
  const [warehouses, setWarehouses] = useState([]);
  const [products, setProducts] = useState([]);
  const [selectedWarehouse, setSelectedWarehouse] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState({ productId: '', warehouseId: '', physicalQuantity: '', reason: '' });
  const [saving, setSaving] = useState(false);

  const loadAll = async (warehouseId) => {
    setLoading(true);
    try {
      const [invRes, whRes, prodRes] = await Promise.all([
        getInventory(warehouseId),
        getWarehouses(),
        getProducts(),
      ]);
      setInventory(invRes.data);
      setWarehouses(whRes.data);
      setProducts(prodRes.data);
    } catch (err) {
      setError('Failed to load inventory.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAll();
  }, []);

  const handleFilterChange = (e) => {
    const value = e.target.value;
    setSelectedWarehouse(value);
    loadAll(value || undefined);
  };

  const handleOpen = () => {
    setFormData({ productId: '', warehouseId: '', physicalQuantity: '', reason: '' });
    setError('');
    setOpen(true);
  };

  const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = async () => {
    if (!formData.productId || !formData.warehouseId || formData.physicalQuantity === '') {
      setError('Product, Warehouse and Physical Quantity are required.');
      return;
    }
    setSaving(true);
    setError('');
    try {
      await adjustStock({
        productId: formData.productId,
        warehouseId: formData.warehouseId,
        physicalQuantity: parseInt(formData.physicalQuantity),
        reason: formData.reason || null,
      });
      setOpen(false);
      loadAll(selectedWarehouse || undefined);
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to adjust stock.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h4" fontWeight={700}>Inventory</Typography>
            <Typography variant="body2" color="text.secondary">Current stock across all warehouses</Typography>
          </Box>
          <Button variant="contained" startIcon={<Tune />} onClick={handleOpen}>
            Adjust Stock
          </Button>
        </Box>

        <TextField
          select
          size="small"
          label="Filter by Warehouse"
          value={selectedWarehouse}
          onChange={handleFilterChange}
          sx={{ mb: 3, minWidth: 260 }}
        >
          <MenuItem value="">All Warehouses</MenuItem>
          {warehouses.map((w) => (
            <MenuItem key={w.warehouseId} value={w.warehouseId}>{w.name}</MenuItem>
          ))}
        </TextField>

        {error && !open && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Paper elevation={0} sx={{ borderRadius: 3, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Product</TableCell>
                  <TableCell>SKU</TableCell>
                  <TableCell>Warehouse</TableCell>
                  <TableCell>Available</TableCell>
                  <TableCell>Reserved</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow><TableCell colSpan={5} align="center" sx={{ py: 4 }}><CircularProgress size={28} /></TableCell></TableRow>
                ) : inventory.length === 0 ? (
                  <TableRow><TableCell colSpan={5} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No stock records yet. Use "Adjust Stock" to add opening stock.</Typography></TableCell></TableRow>
                ) : (
                  inventory.map((inv) => (
                    <TableRow key={inv.inventoryId} hover>
                      <TableCell>{inv.productName}</TableCell>
                      <TableCell>{inv.productSKU}</TableCell>
                      <TableCell>{inv.warehouseName}</TableCell>
                      <TableCell>
                        <Chip
                          label={inv.quantityAvailable}
                          size="small"
                          color={inv.quantityAvailable === 0 ? 'error' : 'success'}
                        />
                      </TableCell>
                      <TableCell>{inv.quantityReserved}</TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </motion.div>

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>Adjust Stock</DialogTitle>
        <DialogContent>
          {error && <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{error}</Alert>}
          <TextField fullWidth select label="Product" name="productId" value={formData.productId} onChange={handleChange} sx={{ mt: 1, mb: 3 }}>
            {products.map((p) => (
              <MenuItem key={p.productId} value={p.productId}>{p.name} ({p.sku})</MenuItem>
            ))}
          </TextField>
          <TextField fullWidth select label="Warehouse" name="warehouseId" value={formData.warehouseId} onChange={handleChange} sx={{ mb: 3 }}>
            {warehouses.map((w) => (
              <MenuItem key={w.warehouseId} value={w.warehouseId}>{w.name}</MenuItem>
            ))}
          </TextField>
          <TextField
            fullWidth
            label="Physical Quantity (actual counted stock)"
            name="physicalQuantity"
            type="number"
            value={formData.physicalQuantity}
            onChange={handleChange}
            sx={{ mb: 3 }}
          />
          <TextField fullWidth label="Reason (optional)" name="reason" value={formData.reason} onChange={handleChange} placeholder="e.g. Opening stock, Physical count, Damaged" />
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 1 }}>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit} disabled={saving}>
            {saving ? <CircularProgress size={20} color="inherit" /> : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
    </DashboardLayout>
  );
}

export default Inventory;