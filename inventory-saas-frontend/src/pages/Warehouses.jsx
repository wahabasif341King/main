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
  Grid,
  Alert,
  CircularProgress,
  IconButton,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import { getWarehouses, createWarehouse, updateWarehouse, deleteWarehouse } from '../api/inventoryApi.js';

const emptyForm = { name: '', code: '', address: '', contactNumber: '' };

function Warehouses() {
  const [warehouses, setWarehouses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState(emptyForm);
  const [editingId, setEditingId] = useState(null);
  const [saving, setSaving] = useState(false);

  const loadWarehouses = async () => {
    setLoading(true);
    try {
      const res = await getWarehouses();
      setWarehouses(res.data);
    } catch (err) {
      setError('Failed to load warehouses.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadWarehouses();
  }, []);

  
    const handleOpen = (warehouse = null) => {
    if (warehouse) {
        setEditingId(warehouse.warehouseId);
        setFormData({
        name: warehouse.name,
        code: warehouse.code || '',
        address: warehouse.address || '',
        contactNumber: warehouse.contactNumber || '',
        });
    } else {
        setEditingId(null);
        setFormData(emptyForm);
    }
    setError('');
    setOpen(true);
    };

  const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

    const handleSubmit = async () => {
    if (!formData.name.trim()) {
        setError('Warehouse name is required.');
        return;
    }
    setSaving(true);
    setError('');
    try {
        const payload = {
        name: formData.name,
        code: formData.code || null,
        address: formData.address || null,
        contactNumber: formData.contactNumber || null,
        };
        if (editingId) {
        await updateWarehouse(editingId, payload);
        } else {
        await createWarehouse(payload);
        }
        setOpen(false);
        setEditingId(null);
        loadWarehouses();
    } catch (err) {
        setError(err.response?.data?.message || err.response?.data || 'Failed to save warehouse.');
    } finally {
        setSaving(false);
    }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Delete this warehouse?')) return;
        try {
            await deleteWarehouse(id);
            loadWarehouses();
        } catch (err) {
            setError(err.response?.data?.message || err.response?.data || 'Failed to delete warehouse.');
        }
    };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h4" fontWeight={700}>Warehouses</Typography>
            <Typography variant="body2" color="text.secondary">Manage your storage locations</Typography>
          </Box>
          <Button variant="contained" startIcon={<Add />} onClick={handleOpen}>
            Add Warehouse
          </Button>
        </Box>

        {error && !open && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Paper elevation={0} sx={{ borderRadius: 3, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>Code</TableCell>
                  <TableCell>Address</TableCell>
                  <TableCell>Contact</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow><TableCell colSpan={5} align="center" sx={{ py: 4 }}><CircularProgress size={28} /></TableCell></TableRow>
                ) : warehouses.length === 0 ? (
                  <TableRow><TableCell colSpan={6} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No warehouses yet.</Typography></TableCell></TableRow>
                ) : (
                  warehouses.map((w) => (
                    <TableRow key={w.warehouseId} hover>
                        <TableCell>{w.name}</TableCell>
                        <TableCell>{w.code || '—'}</TableCell>
                        <TableCell>{w.address || '—'}</TableCell>
                        <TableCell>{w.contactNumber || '—'}</TableCell>
                        <TableCell><Chip label={w.status} size="small" color={w.status === 'Active' ? 'success' : 'default'} /></TableCell>
                        <TableCell align="right">
                            <IconButton size="small" color="primary" onClick={() => handleOpen(w)}>
                            <Edit fontSize="small" />
                            </IconButton>
                            <IconButton size="small" color="error" onClick={() => handleDelete(w.warehouseId)}>
                            <Delete fontSize="small" />
                            </IconButton>
                        </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </motion.div>

      <Dialog open={open} onClose={() => { setOpen(false); setEditingId(null); }} fullWidth maxWidth="sm">
        <DialogTitle>{editingId ? 'Edit Warehouse' : 'Add Warehouse'}</DialogTitle>
        <DialogContent>
          {error && <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{error}</Alert>}
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12} sm={8}>
              <TextField fullWidth label="Warehouse Name" name="name" value={formData.name} onChange={handleChange} required />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField fullWidth label="Code" name="code" value={formData.code} onChange={handleChange} />
            </Grid>
            <Grid item xs={12}>
              <TextField fullWidth label="Address" name="address" value={formData.address} onChange={handleChange} />
            </Grid>
            <Grid item xs={12}>
              <TextField fullWidth label="Contact Number" name="contactNumber" value={formData.contactNumber} onChange={handleChange} />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 1 }}>
          <Button onClick={() => { setOpen(false); setEditingId(null); }}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit} disabled={saving}>
            {saving ? <CircularProgress size={20} color="inherit" /> : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
    </DashboardLayout>
  );
}

export default Warehouses;