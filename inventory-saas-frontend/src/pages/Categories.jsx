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
  IconButton,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Add, Delete, Edit } from '@mui/icons-material';
import { motion } from 'framer-motion';
import DashboardLayout from '../layouts/DashboardLayout.jsx';
import { getCategories, createCategory, updateCategory, deleteCategory } from '../api/catalogApi.js';
function Categories() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState({ name: '', parentCategoryId: '' });
  const [editingId, setEditingId] = useState(null);
  const [saving, setSaving] = useState(false);

  const loadCategories = async () => {
    setLoading(true);
    try {
      const res = await getCategories();
      setCategories(res.data);
    } catch (err) {
      setError('Failed to load categories.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

    const handleOpen = (category = null) => {
    if (category) {
        setEditingId(category.categoryId);
        setFormData({
        name: category.name,
        parentCategoryId: category.parentCategoryId || '',
        });
    } else {
        setEditingId(null);
        setFormData({ name: '', parentCategoryId: '' });
    }
    setError('');
    setOpen(true);
    };

    const handleClose = () => {
    setOpen(false);
    setEditingId(null);
    };

    const handleSubmit = async () => {
    if (!formData.name.trim()) {
        setError('Category name is required.');
        return;
    }
    setSaving(true);
    setError('');
    try {
        const payload = {
        name: formData.name,
        parentCategoryId: formData.parentCategoryId || null,
        };
        if (editingId) {
        await updateCategory(editingId, payload);
        } else {
        await createCategory(payload);
        }
        handleClose();
        loadCategories();
    } catch (err) {
        setError(err.response?.data?.message || err.response?.data || 'Failed to save category.');
    } finally {
        setSaving(false);
    }
    };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this category?')) return;
    try {
      await deleteCategory(id);
      loadCategories();
    } catch (err) {
      setError('Failed to delete category.');
    }
  };

  return (
    <DashboardLayout>
      <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Box>
            <Typography variant="h4" fontWeight={700}>Categories</Typography>
            <Typography variant="body2" color="text.secondary">Organize your products into categories</Typography>
          </Box>
          <Button variant="contained" startIcon={<Add />} onClick={handleOpen}>
            Add Category
          </Button>
        </Box>

        {error && !open && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Paper elevation={0} sx={{ borderRadius: 3, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>Parent Category</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow>
                    <TableCell colSpan={4} align="center" sx={{ py: 4 }}>
                      <CircularProgress size={28} />
                    </TableCell>
                  </TableRow>
                ) : categories.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={4} align="center" sx={{ py: 4 }}>
                      <Typography color="text.secondary">No categories yet. Add your first one.</Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  categories.map((cat) => {
                    const parent = categories.find((c) => c.categoryId === cat.parentCategoryId);
                    return (
                      <TableRow key={cat.categoryId} hover>
                        <TableCell>{cat.name}</TableCell>
                        <TableCell>{parent ? parent.name : '—'}</TableCell>
                        <TableCell>
                          <Chip
                            label={cat.status}
                            size="small"
                            color={cat.status === 'Active' ? 'success' : 'default'}
                          />
                        </TableCell>
                        <TableCell align="right">
                            <IconButton size="small" color="primary" onClick={() => handleOpen(cat)}>
                                <Edit fontSize="small" />
                            </IconButton>
                            <IconButton size="small" color="error" onClick={() => handleDelete(cat.categoryId)}>
                                <Delete fontSize="small" />
                            </IconButton>
                        </TableCell>
                      </TableRow>
                    );
                  })
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </motion.div>

      {/* Add Category Dialog */}
      <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
        <DialogTitle>{editingId ? 'Edit Category' : 'Add Category'}</DialogTitle>
        <DialogContent>
          {error && <Alert severity="error" sx={{ mb: 2, mt: 1 }}>{error}</Alert>}
          <TextField
            fullWidth
            autoFocus
            label="Category Name"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            sx={{ mt: 1, mb: 3 }}
          />
          <TextField
            fullWidth
            select
            label="Parent Category (optional)"
            value={formData.parentCategoryId}
            onChange={(e) => setFormData({ ...formData, parentCategoryId: e.target.value })}
          >
            <MenuItem value="">None</MenuItem>
            {categories.map((cat) => (
              <MenuItem key={cat.categoryId} value={cat.categoryId}>
                {cat.name}
              </MenuItem>
            ))}
          </TextField>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 1 }}>
          <Button onClick={handleClose}>Cancel</Button>
          <Button variant="contained" onClick={handleSubmit} disabled={saving}>
            {saving ? <CircularProgress size={20} color="inherit" /> : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
    </DashboardLayout>
  );
}

export default Categories;