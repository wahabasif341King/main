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
  IconButton,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import { getBrands, createBrand, updateBrand, deleteBrand } from '../api/catalogApi.js';

function Brands() {
  const [brands, setBrands] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState(null); // null = creating, otherwise editing
  const [name, setName] = useState('');
  const [saving, setSaving] = useState(false);

  const loadBrands = async () => {
    setLoading(true);
    try {
      const res = await getBrands();
      setBrands(res.data);
    } catch (err) {
      setError('Failed to load brands.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBrands();
  }, []);

  const handleOpenCreate = () => {
    setEditingId(null);
    setName('');
    setError('');
    setOpen(true);
  };

  const handleOpenEdit = (brand) => {
    setEditingId(brand.brandId);
    setName(brand.name);
    setError('');
    setOpen(true);
  };

  const handleSubmit = async () => {
    if (!name.trim()) {
      setError('Brand name is required.');
      return;
    }
    setSaving(true);
    setError('');
    try {
      if (editingId) {
        await updateBrand(editingId, { name });
      } else {
        await createBrand({ name });
      }
      setOpen(false);
      loadBrands();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to save brand.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this brand?')) return;
    try {
      await deleteBrand(id);
      loadBrands();
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data || 'Failed to delete brand.');
    }
  };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h4" fontWeight={700}>Brands</Typography>
            <Typography variant="body2" color="text.secondary">Manage product brands</Typography>
          </Box>
          <Button variant="contained" startIcon={<Add />} onClick={handleOpenCreate}>
            Add Brand
          </Button>
        </Box>

        {error && !open && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Paper elevation={0} sx={{ borderRadius: 3, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow><TableCell colSpan={3} align="center" sx={{ py: 4 }}><CircularProgress size={28} /></TableCell></TableRow>
                ) : brands.length === 0 ? (
                  <TableRow><TableCell colSpan={3} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No brands yet. Add your first one.</Typography></TableCell></TableRow>
                ) : (
                  brands.map((brand) => (
                    <TableRow key={brand.brandId} hover>
                      <TableCell>{brand.name}</TableCell>
                      <TableCell>
                        <Chip label={brand.status} size="small" color={brand.status === 'Active' ? 'success' : 'default'} />
                      </TableCell>
                      <TableCell align="right">
                        <IconButton size="small" onClick={() => handleOpenEdit(brand)}>
                          <Edit fontSize="small" />
                        </IconButton>
                        <IconButton size="small" color="error" onClick={() => handleDelete(brand.brandId)}>
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
        <DialogTitle>{editingId ? 'Edit Brand' : 'Add Brand'}</DialogTitle>
        <DialogContent>
          {error && <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{error}</Alert>}
          <TextField
            fullWidth
            autoFocus
            label="Brand Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            sx={{ mt: 1 }}
          />
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

export default Brands;