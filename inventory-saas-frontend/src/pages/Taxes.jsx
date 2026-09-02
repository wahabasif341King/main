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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  IconButton,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import { getTaxes, createTax, updateTax, deleteTax } from '../api/catalogApi.js';

const emptyForm = { name: '', percentage: '', appliesTo: 'Product' };

function Taxes() {
  const [taxes, setTaxes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState(emptyForm);
  const [saving, setSaving] = useState(false);

  const loadTaxes = async () => {
    setLoading(true);
    try {
      const res = await getTaxes();
      setTaxes(res.data);
    } catch (err) {
      setError('Failed to load taxes.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadTaxes();
  }, []);

  const handleOpenCreate = () => {
    setEditingId(null);
    setFormData(emptyForm);
    setError('');
    setOpen(true);
  };

  const handleOpenEdit = (tax) => {
    setEditingId(tax.taxId);
    setFormData({ name: tax.name, percentage: tax.percentage, appliesTo: tax.appliesTo });
    setError('');
    setOpen(true);
  };

  const handleSubmit = async () => {
    if (!formData.name.trim() || formData.percentage === '') {
      setError('Name and percentage are required.');
      return;
    }
    setSaving(true);
    setError('');
    const payload = {
      name: formData.name,
      percentage: parseFloat(formData.percentage),
      appliesTo: formData.appliesTo,
    };
    try {
      if (editingId) {
        await updateTax(editingId, payload);
      } else {
        await createTax(payload);
      }
      setOpen(false);
      loadTaxes();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to save tax.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this tax?')) return;
    try {
      await deleteTax(id);
      loadTaxes();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'This tax may still be assigned to a product.');
    }
  };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h4" fontWeight={700}>Taxes</Typography>
            <Typography variant="body2" color="text.secondary">Configure tax rates (GST, VAT, Sales Tax...)</Typography>
          </Box>
          <Button variant="contained" startIcon={<Add />} onClick={handleOpenCreate}>
            Add Tax
          </Button>
        </Box>

        {error && !open && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Paper elevation={0} sx={{ borderRadius: 3, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>Percentage</TableCell>
                  <TableCell>Applies To</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow><TableCell colSpan={4} align="center" sx={{ py: 4 }}><CircularProgress size={28} /></TableCell></TableRow>
                ) : taxes.length === 0 ? (
                  <TableRow><TableCell colSpan={4} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No taxes configured yet.</Typography></TableCell></TableRow>
                ) : (
                  taxes.map((tax) => (
                    <TableRow key={tax.taxId} hover>
                      <TableCell>{tax.name}</TableCell>
                      <TableCell>{tax.percentage}%</TableCell>
                      <TableCell>{tax.appliesTo}</TableCell>
                      <TableCell align="right">
                        <IconButton size="small" onClick={() => handleOpenEdit(tax)}>
                          <Edit fontSize="small" />
                        </IconButton>
                        <IconButton size="small" color="error" onClick={() => handleDelete(tax.taxId)}>
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

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{editingId ? 'Edit Tax' : 'Add Tax'}</DialogTitle>
        <DialogContent>
          {error && <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{error}</Alert>}
          <TextField
            fullWidth
            autoFocus
            label="Tax Name (e.g. GST)"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            sx={{ mt: 1, mb: 3 }}
          />
          <TextField
            fullWidth
            label="Percentage"
            type="number"
            value={formData.percentage}
            onChange={(e) => setFormData({ ...formData, percentage: e.target.value })}
            sx={{ mb: 3 }}
          />
          <TextField
            fullWidth
            select
            label="Applies To"
            value={formData.appliesTo}
            onChange={(e) => setFormData({ ...formData, appliesTo: e.target.value })}
          >
            <MenuItem value="Product">Product</MenuItem>
            <MenuItem value="Customer">Customer</MenuItem>
            <MenuItem value="Order">Order</MenuItem>
          </TextField>
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

export default Taxes;